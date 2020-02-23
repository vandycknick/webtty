$ARTIFACTS = "$(Get-Location)\artifacts"

dotnet tool install --global --add-source $ARTIFACTS `
    --version $(dotnet minver -t v -a minor -v e) `
    webtty
