.PHONY: clean watch watch-client watch-server
.DEFAULT_GOAL := default

CONFIGURATION	:= Debug

SRC_UI			= src/WebTty.UI
SRC_CMD			= src/WebTty/WebTty.csproj
SRC_NATIVE		= src/WebTty.Native/WebTty.Native.csproj
ARTIFACTS		= $(shell pwd)/artifacts
CLI_TOOL		= webtty

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

watch: pack-native
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
		-property:VersionSuffix=beta.1

package: build-ui pack-native
	dotnet pack $(SRC_CMD) \
		--configuration $(CONFIGURATION) \
		--output $(ARTIFACTS)
