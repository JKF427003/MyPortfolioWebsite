param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("Test", "Production")]
    [string]$Environment
)

$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
$publishFolder = Join-Path $projectRoot "artifacts\publish-$Environment"

if ($Environment -eq "Test") {
    $deployFolder = "C:\Deployments\MyPortfolioWebsite-Test"
}
else {
    $deployFolder = "C:\Deployments\MyPortfolioWebsite-Production"
}

Write-Host "Publishing application for $Environment..."
dotnet publish "$projectRoot\MyPortfolioWebsite.csproj" `
    --configuration Release `
    --output $publishFolder

Write-Host "Preparing deployment folder: $deployFolder"
New-Item -ItemType Directory -Force -Path $deployFolder | Out-Null

Write-Host "Copying published files..."
Copy-Item "$publishFolder\*" $deployFolder -Recurse -Force

Write-Host "Deployment completed."
Write-Host "Environment: $Environment"
Write-Host "Deployment folder: $deployFolder"