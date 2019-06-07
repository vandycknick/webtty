.PHONY: clean build dev watch watch-client watch-server

SRC_UI=src/WebTty.UI
SRC_CMD=src/WebTty

setup:
	yarn --cwd ${SRC_UI} install
	dotnet restore

clean:
	yarn --cwd ${SRC_UI} run clean

build: clean
	yarn --cwd ${SRC_UI} run build

dev:
	make watch -j2

watch-client:
	direnv allow && \
	yarn --cwd ${SRC_UI} run watch

watch-server:
	direnv allow && \
	dotnet watch -p ${SRC_CMD} run

watch: watch-client watch-server
