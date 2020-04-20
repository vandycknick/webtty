dotnet restore
dotnet tool restore

dotnet build "$(Get-Location)\src\WebTty.Schema\WebTty.Schema.csproj"
dotnet run -p tools/jsonschema/jsonschema.csproj -- `
    --assembly "$(Get-Location)\.build\bin\WebTty.Schema\Debug\netstandard2.0\WebTty.Schema.dll" `
    --namespace "WebTty.Schema.Messages" `
    --output "$(Get-Location)\src\WebTty.Hosting\Client\.tmp\messages"
