$CLI_PROJECT = "$(Get-Location)\src\webTty\webTty.csproj"
$CONFIGURATION = "Release"
$ARTIFACTS = "$(Get-Location)\artifacts"

dotnet restore --force-evaluate
dotnet build --configuration $CONFIGURATION /property:IsPackaging=True $CLI_PROJECT
dotnet pack $CLI_PROJECT --configuration $CONFIGURATION `
    --no-build `
    --output $ARTIFACTS `
    --include-symbols
