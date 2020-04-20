FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201 AS dev

WORKDIR /app

RUN curl -sL https://deb.nodesource.com/setup_12.x | bash - && \
    curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | apt-key add - && \
    echo "deb https://dl.yarnpkg.com/debian/ stable main" | tee /etc/apt/sources.list.d/yarn.list && \
    apt update && \
    apt install -y --no-install-recommends nodejs make yarn apt-transport-https && \
    rm -rf /var/lib/apt/lists/*

# COPY sln, prop files and makefile
COPY ./*.sln ./Common.props ./Directory.Build.props ./Makefile ./

# Install node modules
COPY ./src/WebTty.Hosting/Client/package.json \
    ./src/WebTty.Hosting/Client/yarn.lock \
    ./src/WebTty.Hosting/Client/Client.targets \
    ./src/WebTty.Hosting/Client/
RUN cd src/WebTty.Hosting/Client && yarn --frozen-lockfile --production=false && yarn cache clean

# Copy the main source project files
COPY src/*/*.csproj ./
COPY src/WebTty.Exec/Directory.Build.props src/WebTty.Exec/Sources.targets ./src/WebTty.Exec/
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# Copy the test project files
COPY test/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p test/${file%.*}/ && mv $file test/${file%.*}/; done

# COPY tools
COPY tools/jsonschema tools/jsonschema/

RUN dotnet restore

COPY . .

RUN make setup

FROM dev as build

RUN make package

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-alpine as runtime
ENV ASPNETCORE_URLS=""
WORKDIR /webtty

COPY --from=build /app/.build/bin/WebTty/Release/netcoreapp3.1/ .

ENTRYPOINT ["dotnet", "webtty.dll", "-a", "any"]
