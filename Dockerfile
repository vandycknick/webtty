FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101 AS build

WORKDIR /app

RUN apt update && apt install -y sudo apt-transport-https

RUN curl -sL https://deb.nodesource.com/setup_12.x | bash -
RUN curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add -
RUN echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list
RUN apt update && apt install -y nodejs make yarn

# COPY sln and prop files
COPY webtty.sln .
COPY BuildOutput.props .
COPY Directory.Build.props .

# COPY src
COPY src/WebTty/*.csproj src/WebTty/
COPY src/WebTty.Exec/*.csproj src/WebTty.Exec/
COPY src/WebTty.Api/*.csproj src/WebTty.Api/

COPY src/WebTty.Hosting/Client/package.json src/WebTty.Hosting/Client
COPY src/WebTty.Hosting/Client/yarn.lock src/WebTty.Hosting/Client
COPY src/WebTty.Hosting/*.csproj src/WebTty.Hosting/

# COPY test
COPY test/WebTty.Test/*.csproj test/WebTty.Test/

# COPY tools
COPY tools/build tools/build/
COPY tools/jsonschema tools/jsonschema/

# COPY nuke
COPY build.sh .
COPY .nuke .

RUN ./build.sh restore --no-logo

COPY . .

RUN ./build.sh setup --no-logo
RUN ./build.sh compile --configuration Release --no-logo

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-alpine as runtime
ENV ASPNETCORE_URLS=""
WORKDIR /webtty

COPY --from=build /app/.build/bin/WebTty/Release/netcoreapp3.1/ .

ENTRYPOINT ["dotnet", "webtty.dll", "-a", "any"]
