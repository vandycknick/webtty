.PHONY: clean build dev watch watch-client watch-server

CONFIGURATION	:= Debug

SRC_UI			= src/WebTty.UI
SRC_CMD			= src/WebTty

setup:
	yarn --cwd ${SRC_UI} install
	dotnet restore

clean:
	yarn --cwd ${SRC_UI} run clean

build: clean
	yarn --cwd ${SRC_UI} run build

dev:
	$(MAKE) watch

watch-client:
	direnv allow && \
	yarn --cwd ${SRC_UI} run watch

watch-server:
	direnv allow && \
	dotnet watch -p ${SRC_CMD} run

pack-native:
	dotnet pack src/WebTty.Native/WebTty.Native.csproj \
		-c $(CONFIGURATION) \
		--output ../../artifacts \
		-property:BuildNumber=1

watch: pack-native
	$(MAKE) -j2 watch-client watch-server
