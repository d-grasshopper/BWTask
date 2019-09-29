param (
    [Parameter(Mandatory = $true)][string]$subscription,
    [Parameter(Mandatory = $true)][string]$environment,
    [string[]]$appsToDeploy = "all",
    [switch]$skipUpdate = $false,
    [switch]${forward-integration} = $false
)

function CheckoutOrUpdate {
    param (
        [Parameter(Mandatory = $true)][string]$repository,
        [Parameter(Mandatory = $true)][string]$commitId,
        [Parameter(Mandatory = $true)][string]$id,
        [boolean]$skipUpdate = $false,
        [Parameter(mandatory = $true)][boolean]${forward-integration} = $false
    )

    if (! (Test-Path $id)) {
        git clone $repository $id
        if (!$?) {
            Write-Warning "Cloning repo ${repository} failed!"
            continue
        }

        Set-Location $id
        git checkout $commitId
    }
    elseif (!skipUpdate) {
        Write-Output "Already checked out, updating"
        Set-Location $id
        git fetch --all
        git reset --hard
        git checkout $commitId
        git pull
    }
    else {
        Write-Output "Skipping update"
        Set-Location $id
    }

    if (${forward-integration}) {
        Write-Output "Forward-integrating"
        $branch_name = git rev-parse --abbrev-ref HEAD
        git fetch --all
        git checkout master
        git pull
        git checkout $branch_name
        git merge master
    }
}

function BuildImage {
    param (
        [Parameter(Mandatory = $true)][string]$subscription,
        [Parameter(Mandatory = $true)][string]$environment,
        [Parameter(Mandatory = $true)][string]$webProject,
        [Parameter(Mandatory = $true)][string]$appId,
        [Parameter(Mandatory = $true)][string]$registry
    )

    $latestTag = docker run -it --rm -v ${subscription}_azure_credentials:/root/.azure/ microsoft/azure-cli:latest az acr repository show-tags -n ${registry} --repository acr_${appId} --top 1 --orderby time_desc
    docker build -t ${appId}_image:v1 ./${appId}
}

function Deploy {
    param (
        [Parameter(Mandatory = $true)][string]$subscription,
        [Parameter(Mandatory = $true)][string]$environment,
        [Parameter(Mandatory = $true)][string]$webProject,
        [Parameter(Mandatory = $true)][string]$appId,
        [Parameter(Mandatory = $true)][string]$reposPath
    )

    # Get settings from terraform for app
    $terraform_state = docker run --rm -v ${subscription}_terraform:/terraform alpine cat /terraform/${subscription}_${environment}/terraform.tfstate | ConvertFrom-Json
    $appSettings = $terraform_state.modules[0].outputs."${appId}_credentials".value[0]
    $appServiceId = $terraform_state.modules[0].outputs."${appId}_name".value

    # Publish package
    Set-Location $reposPath\Deployment\$appId
    $deployCommand = "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe"
    $packageToDeploy = "$reposPath\Deployment\$appId\$webProject.zip"
    $parameterFile = "$reposPath\Deployment\$appId\$webProject.SetParameters.xml"
    $webAppName = ' -setParam:name="IIS Web Application Name",value=' + $appServiceId

    # Stop App Service before deployment
    docker run -it --rm -v ${subscription}_azure_credentials:/root/.azure/ microsoft/azure-cli:latest az webapp stop --name $appServiceId --resource-group ${subscription}_${environment}
    @$deployCommand -source:package=$packageToDeploy -dest:"auto,computerName=https://$($appServiceId).scm.azurewebsites.net:443/msdeploy.axd?site=$($appServiceId),userName=$($appSettings.username),password=$($appSettings.password),authtype=Basic,includeAcls=False" -verb:sync -disableLink:ContentExtension -disableLink:CertificateExtension -setParamFile:$parameterFile $webAppName
    Write-Output "Deployment ended with code: $?"

    # Start App Service after deployment
    docker run -it --rm -v ${subscription}_azure_credentials:/root/.azure/ microsoft/azure-cli:latest az webapp start --name $appServiceId --resource-group ${subscription}_${environment}
}

function DeployApp {
    param (
        [Parameter(Mandatory = $true)][string]$subscription,
        [Parameter(Mandatory = $true)][string]$environment,
        [Parameter(Mandatory = $true)][PSCustomObject]$app,
        [Parameter(Mandatory = $true)][string]$currentPath,
        [Parameter(Mandatory = $true)][string]$msbuildPath,
        [Parameter(Mandatory = $true)][string]$reposPath,
        [string]$requiredCommitId = "",
        [string]$requiredBuildProfile = "",
        [Parameter(Mandatory = $true)][boolean]$skipUpdate = $false,
        [Parameter(Mandatory = $true)][boolean]$forwardIntegration
    )

    Write-Output "Processing app: $(app)"

    # Checkout source
    if ($requiredCommitId -eq "") {
        CheckoutOrUpdate -repository $app.repo -commitId $app.commitId -id $app.id -skipUpdate $skipUpdate $forwardIntegration
    }
    else {
        CheckoutOrUpdate -repository $app.repo -commitId $requiredCommitId -id $app.id -skipUpdate $skipUpdate $forwardIntegration
    }

    # Compile source
    if ($requiredBuildProfile -eq "") {
        # Compile -currentPath $currentPath -msbuildPath $msbuildPath -solution $app.solution -appId $app.id -reposPath $reposPath -buildProfile "release"
        BuildImages
    }
    else {
        # Compile -currentPath $currentPath -msbuildPath $msbuildPath -solution $app.solution -appId $app.id -reposPath $reposPath -buildProfile $requiredBuildProfile
        BuildImages
    }

    # Deploy
    Deploy -subscription $subscription -environment $environment -webProject $app.webProject -appId $app.id -reposPath $reposPath

    Set-Location $reposPath
}

$currentPath = Get-Location
$configuration = (Get-Content configuration.json -Raw) | ConvertFrom-Json
if (!(Test-Path "\repositories")) {
    mkdir \repositories
}
Set-Location \repositories
$reposPath = Get-Location

# Fetch nuget
$uri = "https://dist.nuget.org/win-x86-commandline/v$($configuration.nugetVersion)/nuget.exe"
if (!(Test-Path "$currentPath\nuget\NuGet.exe")) {
    Invoke-WebRequest -Uri $uri -OutFile "$currentPath\nuget\NuGet.exe"
}
$forwardIntegration = $false
if (${forward-integration}) {
    $forwardIntegration = $true
}

if ($appsToDeploy.Count -eq 0) {
    Write-Output "No apps to deploy"
}
elseif ($appsToDeploy[0] -eq "all") {
    foreach ($app in $configuration.apps) {
        DeployApp -subscription $subscription -environment $environment -app $app -currentPath $currentPath -msbuildPath $configuration.msbuildPath -reposPath $reposPath -skipUpdate $skipUpdate -forwardIntegration $forwardIntegration
    }
}
else {
    foreach ($app in $configuration.apps) {
        foreach ($appId in $appsToDeploy) {
            $nameParts = $appId -split ":"
            if ($nameParts[0] -eq $app.id) {
                DeployApp -subscription $subscription -environment $environment -app $app -currentPath $currentPath -msbuildPath $configuration.msbuildPath -reposPath $reposPath -requiredCommitId $nameParts[1] -requiredBuildProfile $nameParts[2] -skipUpdate $skipUpdate -forwardIntegration ${forward-integration}
            }
            else {
                Write-Output "Skipping app: ${app.id}"
            }
        }
    }
}
Set-Location $currentPath
