param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

git checkout main
git pull origin main
git tag "v$Version"
git push origin "v$Version"

Write-Host "Tag v$Version pushed - publish.yml should now be running." -ForegroundColor Green