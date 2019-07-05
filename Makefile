.PHONY: clean watch watch-client watch-server test
.DEFAULT_GOAL := default

CONFIGURATION	:= Debug

SRC_UI			= src/WebTty.UI
SRC_CMD			= src/WebTty/WebTty.csproj
SRC_NATIVE		= src/WebTty.Native/WebTty.Native.csproj
ARTIFACTS		= $(shell pwd)/artifacts
CLI_TOOL		= webtty
NATIVE_SUFFIX	:=build.$(shell date "+%Y%m%d%H%M%S")

default: setup clean
	$(MAKE) package CONFIGURATION=Release

install:
	dotnet tool install -g --add-source $(ARTIFACTS) $(CLI_TOOL)

uninstall:
	dotnet tool uninstall -g $(CLI_TOOL)

setup:
	yarn --cwd $(SRC_UI) install
	dotnet restore

clean:
	rm -rf $(ARTIFACTS)
	yarn --cwd $(SRC_UI) run clean

test:
	dotnet test test/WebTty.Test
	dotnet test test/WebTty.Integration.Test

watch:
	$(MAKE) -j2 watch-client watch-server

watch-client:
	direnv allow && \
	yarn --cwd $(SRC_UI) run watch

watch-server:
	direnv allow && \
	dotnet watch -p $(SRC_CMD) run

build-ui:
	yarn --cwd $(SRC_UI) run build

pack-native:
	dotnet pack $(SRC_NATIVE) \
		--configuration $(CONFIGURATION) \
		--output $(ARTIFACTS) \
		--version-suffix $(NATIVE_SUFFIX)

package: build-ui pack-native
	dotnet restore --force-evaluate
	dotnet build $(SRC_CMD) --configuration $(CONFIGURATION) -p:IsPackaging=true
	dotnet pack $(SRC_CMD) --configuration $(CONFIGURATION) --no-build --output $(ARTIFACTS)

package-release:
	$(MAKE) package CONFIGURATION=Release
