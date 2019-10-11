<h1 align="center">
    <img src="assets/logo.png" width="200" alt="webtty logo" />
    <br />
    <br />
</h1>

> Terminal emulator build on top of WebSockets

[![Build Status][azure-ci-badge]][azure-ci-url]


## Introduction
WebTTY is a cross-platform CLI tool allowing a user to get access to a terminal via any supported browser.

## Install

You can easily install the application as a dotnet tool with the following command (for this you will need to have the dotnet [.NET Core SDK](https://dotnet.microsoft.com/download) installed):

```
dotnet tool install -g webtty
```

## Usage

```
λ webtty --help

Usage: webtty [options]

Options

  -p, --port=VALUE           Port to use [5000]. Use 0 for a dynamic port.
      --version              Show current version
  -?, -h, --help             Show help information
```

[azure-ci-badge]: https://dev.azure.com/vandycknick/webtty/_apis/build/status/nickvdyck.webtty?branchName=master
[azure-ci-url]: https://dev.azure.com/vandycknick/webtty/_build/latest?definitionId=15&branchName=master
