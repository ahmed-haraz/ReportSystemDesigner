# Build script for ReportSystem
param(
    [string]$Configuration = "Release",
    [string]$OutputPath = "./artifacts"
)

$ErrorActionPreference = "Stop"

Write-Host "Building ReportSystem..." -ForegroundColor Green

# Create output directory
New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null

# Build Core libraries
Write-Host "Building Core libraries..." -ForegroundColor Cyan
dotnet build "./ReportDesigner/ReportDesigner.Core/ReportDesigner.Core.csproj" -c $Configuration
dotnet build "./MauiReportEngine/MauiReportEngine.Core/MauiReportEngine.Core.csproj" -c $Configuration
dotnet build "./MauiReportEngine/MauiReportEngine.Renderer/MauiReportEngine.Renderer.csproj" -c $Configuration

# Build Desktop Designer
Write-Host "Building Desktop Designer..." -ForegroundColor Cyan
dotnet build "./ReportDesigner/ReportDesigner.Desktop/ReportDesigner.Desktop.csproj" -c $Configuration

# Build MAUI Engine
Write-Host "Building MAUI Engine..." -ForegroundColor Cyan
dotnet build "./MauiReportEngine/MauiReportEngine.MAUI/MauiReportEngine.MAUI.csproj" -c $Configuration

# Pack NuGet packages
Write-Host "Packing NuGet packages..." -ForegroundColor Cyan
dotnet pack "./ReportDesigner/ReportDesigner.Core/ReportDesigner.Core.csproj" -c $Configuration -o $OutputPath
dotnet pack "./MauiReportEngine/MauiReportEngine.Core/MauiReportEngine.Core.csproj" -c $Configuration -o $OutputPath
dotnet pack "./MauiReportEngine/MauiReportEngine.Renderer/MauiReportEngine.Renderer.csproj" -c $Configuration -o $OutputPath

Write-Host "Build complete! Artifacts in $OutputPath" -ForegroundColor Green
