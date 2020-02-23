dotnet restore
dotnet tool restore

dotnet build "$(Get-Location)\src\WebTty.Api\WebTty.Api.csproj"
dotnet run -p tools/jsonschema/jsonschema.csproj -- `
    --assembly "$(Get-Location)\.build\bin\WebTty.Api\Debug\netcoreapp3.1\WebTty.Api.dll" `
    --namespace "WebTty.Api.Messages" `
    --output "$(Get-Location)\src\WebTty.Hosting\Client\.tmp\messages"
