# Deploy script for ReportSystem
param(
    [string]$Target = "all",
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

Write-Host "Deploying ReportSystem v$Version..." -ForegroundColor Green

switch ($Target) {
    "all" {
        & "./build.ps1" -Configuration Release
        # Copy templates
        Copy-Item -Path "./Shared/Templates" -Destination "./artifacts/Templates" -Recurse -Force
        # Create release package
        Compress-Archive -Path "./artifacts/*" -DestinationPath "./ReportSystem-v$Version.zip" -Force
    }
    "designer" {
        dotnet publish "./ReportDesigner/ReportDesigner.Desktop/ReportDesigner.Desktop.csproj" -c Release -r win-x64 --self-contained
    }
    "maui" {
        dotnet publish "./MauiReportEngine/MauiReportEngine.MAUI/MauiReportEngine.MAUI.csproj" -c Release -f net10.0-android
        dotnet publish "./MauiReportEngine/MauiReportEngine.MAUI/MauiReportEngine.MAUI.csproj" -c Release -f net10.0-ios
    }
}

Write-Host "Deployment complete!" -ForegroundColor Green
