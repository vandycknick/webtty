$ARTIFACTS = "$(Get-Location)\artifacts"
$CLI_TOOL = "webtty"

dotnet tool install --global --add-source $ARTIFACTS `
    --version $(dotnet minver -t v -a minor -v e) `
    $CLI_TOOL
