FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101 AS dev

WORKDIR /app

RUN apt update && apt install -y sudo apt-transport-https

RUN curl -sL https://deb.nodesource.com/setup_12.x | bash -
RUN curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add -
RUN echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list
RUN apt update && apt install -y nodejs make yarn

# COPY sln, prop files and makefile
COPY ./*.sln ./Common.props ./Directory.Build.props ./Makefile ./

# Copy the main source project files
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

COPY ./src/WebTty.Hosting/Client/package.json ./src/WebTty.Hosting/Client/yarn.lock ./src/WebTty.Hosting/Client/

# Copy the test project files
COPY test/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p test/${file%.*}/ && mv $file test/${file%.*}/; done

# COPY tools
COPY tools/jsonschema tools/jsonschema/

RUN dotnet restore
RUN cd src/WebTty.Hosting/Client && yarn

FROM dev as build

COPY . .

RUN make setup
RUN make package

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-alpine as runtime
ENV ASPNETCORE_URLS=""
WORKDIR /webtty

COPY --from=build /app/.build/bin/WebTty/Release/netcoreapp3.1/ .

ENTRYPOINT ["dotnet", "webtty.dll", "-a", "any"]
