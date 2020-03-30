$CLI_PROJECT = "$(Get-Location)\src\webTty\webTty.csproj"
$WEBTTY_EXEC = "$(Get-Location)\src\WebTty.Exec\WebTty.Exec.csproj"
$CONFIGURATION = "Release"
$ARTIFACTS = "$(Get-Location)\artifacts"

dotnet pack $WEBTTY_EXEC `
    --configuration $CONFIGURATION `
    --output $ARTIFACTS `
    --version-suffix build.$(Get-Date -UFormat "+%Y%m%d%H%M%S")

dotnet restore --force-evaluate
dotnet build --configuration $CONFIGURATION /property:IsPackaging=True $CLI_PROJECT
dotnet pack $CLI_PROJECT --configuration $CONFIGURATION `
    --no-build `
    --output $ARTIFACTS `
    --include-symbols
