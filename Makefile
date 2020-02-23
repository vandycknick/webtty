.PHONY: purge clean default install uninstall setup test
.DEFAULT_GOAL := default

ARTIFACTS 		:= $(shell pwd)/artifacts
BUILD			:= $(shell pwd)/.build
TEMP			:= $(shell pwd)/.tmp
CONFIGURATION	:= Release
WEBTTY_EXEC		:= src/WebTty.Exec/WebTty.Exec.csproj
WEBTTY_CLIENT	:= src/WebTty.Hosting/Client
CLI_PROJECT		:= src/WebTty/WebTty.csproj
CLI_TOOL		:= webtty
RUNTIME 		:= linux-x64

purge:
	rm -rf $(BUILD)
	rm -rf $(ARTIFACTS)
	rm -rf $(TEMP)

clean:
	rm -rf $(ARTIFACTS)
	dotnet clean

restore:
	dotnet restore
	dotnet tool restore

setup: restore schema
	dotnet build -c $(CONFIGURATION)

default:
	$(MAKE) package

package: restore package-exec
	dotnet restore --force-evaluate
	dotnet build -c $(CONFIGURATION) /property:IsPackaging=True $(CLI_PROJECT)
	dotnet pack $(CLI_PROJECT) --configuration $(CONFIGURATION) \
		--no-build \
		--output $(ARTIFACTS) \
		--include-symbols

package-exec:
	dotnet pack $(WEBTTY_EXEC) \
		--configuration $(CONFIGURATION) \
		--output $(ARTIFACTS) \
		--version-suffix build.$(shell date "+%Y%m%d%H%M%S")

package-native:
	dotnet publish $(CLI_PROJECT) -c $(CONFIGURATION) \
		--output $(BUILD)/publish/$(RUNTIME) \
		--runtime $(RUNTIME) \
		/property:PublishTrimmed=true \
		/property:PublishSingleFile=true

	@mkdir -p $(ARTIFACTS)
	@cp $(BUILD)/publish/$(RUNTIME)/$(CLI_TOOL) $(ARTIFACTS)/$(CLI_TOOL).$(RUNTIME)

package-osx:
	$(MAKE) package-native RUNTIME=osx-x64

install:
	dotnet tool install --global --add-source $(ARTIFACTS) \
		--version $$(dotnet minver -t v -a minor -v e) \
		$(CLI_TOOL)

uninstall:
	dotnet tool uninstall --global $(CLI_TOOL)

lint: types
	yarn --cwd $(WEBTTY_CLIENT) lint

types:
	yarn --cwd $(WEBTTY_CLIENT) tsc --noEmit

test:
	yarn --cwd $(WEBTTY_CLIENT) test --coverage --coverageDirectory $(TEMP)/webtty.hosting/client

	dotnet test test/WebTty.Test/WebTty.Test.csproj -c Release \
		/property:CollectCoverage=true \
		/property:CoverletOutputFormat=lcov \
		/property:CoverletOutput=$(TEMP)/webtty.test/lcov.info

	dotnet test test/WebTty.Api.Test/WebTty.Api.Test.csproj -c Release \
		/property:CollectCoverage=true \
		/property:CoverletOutputFormat=lcov \
		/property:CoverletOutput=$(TEMP)/webtty.api.test/lcov.info

	dotnet test test/WebTty.Integration.Test/WebTty.Integration.Test.csproj -c Release \
		/property:CollectCoverage=true \
		/property:CoverletOutputFormat=lcov \
		/property:CoverletOutput=$(TEMP)/webtty.integration.test/lcov.info

dev:
	$(MAKE) -j2 dev-server dev-client --ignore-errors

dev-server:
	yarn --cwd $(WEBTTY_CLIENT) watch

dev-client:
	dotnet watch -p $(CLI_PROJECT) run

schema:
	dotnet build $(shell pwd)/src/WebTty.Api/WebTty.Api.csproj
	dotnet run -p tools/jsonschema/jsonschema.csproj -- \
		--assembly $(shell pwd)/.build/bin/WebTty.Api/Debug/netcoreapp3.1/WebTty.Api.dll \
		--namespace WebTty.Api.Messages \
		--output $(shell pwd)/$(WEBTTY_CLIENT)/.tmp/messages
