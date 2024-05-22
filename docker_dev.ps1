Push-Location (Split-Path $MyInvocation.MyCommand.Path)

try {

    Write-Output "Pruning"
    docker container prune -f
    docker volume prune -f
    docker image prune -f

    Write-Output "Building"
    docker build -t caelum-server -f dockerfile .

    Write-Output "Compose Up"
    docker compose up -d
}
catch {
    Write-Host "An error occurred that could not be resolved."
    Write-Host $_.ScriptStackTrace
}

Pop-Location
