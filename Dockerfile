FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101 AS build

WORKDIR /app

RUN apt update && apt install -y sudo apt-transport-https

RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add -
RUN echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list
RUN apt update && apt install -y nodejs make yarn

# COPY solution
COPY webtty.sln .
COPY Directory.Build.props .
COPY Version.props .

# COPY src
COPY src/WebTty/*.csproj src/WebTty/
COPY src/WebTty.Exec/*.csproj src/WebTty.Exec/
COPY src/WebTty.Messages/*.csproj src/WebTty.Messages/

COPY src/WebTty.UI/package.json src/WebTty.UI/
COPY src/WebTty.UI/yarn.lock src/WebTty.UI/
COPY src/WebTty.UI/*.csproj src/WebTty.UI/

# COPY test
COPY test/WebTty.Test/*.csproj test/WebTty.Test/
COPY test/WebTty.Integration.Test/*.csproj test/WebTty.Integration.Test/

# COPY tools
COPY tools/build/* tools/build/
COPY tools/jsonschema/* tools/jsonschema/

# COPY nuke
COPY build.sh .
COPY .nuke .

RUN ./build.sh restore

COPY . .

RUN ./build.sh setup
RUN ./build.sh compile --configuration Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-alpine as runtime
ENV ASPNETCORE_URLS=""
WORKDIR /webtty

COPY --from=build /app/.build/bin/WebTty/Release/netcoreapp3.1/ .

ENTRYPOINT ["dotnet", "webtty.dll", "-a", "any"]
