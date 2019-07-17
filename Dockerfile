FROM mcr.microsoft.com/dotnet/core/sdk:2.2

WORKDIR /app

RUN apt update && apt install -y sudo apt-transport-https

RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add -
RUN echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list
RUN apt update && apt install -y nodejs make yarn

# COPY solution
COPY webtty.sln .

# COPY src
COPY src/WebTty/*.csproj src/WebTty/
COPY src/WebTty.Native/*.csproj src/WebTty.Native/

COPY src/WebTty.UI/package.json src/WebTty.UI/
COPY src/WebTty.UI/yarn.lock src/WebTty.UI/
COPY src/WebTty.UI/*.csproj src/WebTty.UI/

# COPY test
COPY test/WebTty.Test/*.csproj test/WebTty.Test/
COPY test/WebTty.Integration.Test/*.csproj test/WebTty.Integration.Test/

# COPY tools
COPY tools/build/* tools/build/

# COPY nuke
COPY build.sh .
COPY .nuke .

RUN ./build.sh setup

COPY . .

RUN ./build.sh compile
