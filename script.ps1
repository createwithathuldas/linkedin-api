$branches = @(
    "feature/analytics",
    "feature/auth",
    "feature/companies",
    "feature/connections-network",
    "feature/core-infrastructure",
    "feature/feed-posts-media",
    "feature/games",
    "feature/groups",
    "feature/jobs",
    "feature/messaging",
    "feature/notifications",
    "feature/premium",
    "feature/profile",
    "feature/search",
    "feature/settings"
)

foreach ($branch in $branches) {
    Write-Host "`n========================================"
    Write-Host "Checking out $branch"
    Write-Host "========================================"

    git checkout $branch

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to checkout $branch. Skipping..."
        continue
    }

    Write-Host "Pushing $branch to origin..."
    git push -u origin $branch

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to push $branch"
    }
}

Write-Host "`nAll branches processed."