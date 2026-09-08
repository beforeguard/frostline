param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

git checkout main
git pull origin main
git checkout -b "release/$Version"
dotnet versionize --release-as $Version --skip-tag
git push -u origin "release/$Version"

Write-Host "Release branch pushed. Open a PR into main, get it reviewed, and merge it (prefer 'Create a merge commit')." -ForegroundColor Yellow
Write-Host "Then run: .\scripts\release-finish.ps1 -Version $Version" -ForegroundColor Yellow