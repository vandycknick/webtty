<h1 align="center">
    <img src="assets/logo.png" width="200" alt="webtty logo" />
    <br />
    <br />
</h1>

> Terminal emulator build on top of WebSockets

[![Build Status][azure-ci-badge]][azure-ci-url]
[![NuGet][nuget-package-badge]][nuget-package-url]


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

🔌 WebSocket based terminal emulator

Usage: webtty [options] -- [command] [<arguments...>]

Options:
  -a, --address=VALUE        IP address to use [localhost]. Use any to listen
                               to any available address. Ex (0.0.0.0, any, 192.
                               168.2.3, ...)
  -s, --unix-socket=VALUE    Use the given Unix domain socket path for the
                               server to listen to
  -p, --port=VALUE           Port to use [5000]. Use 0 for a dynamic port.
      --path=VALUE           Path to use, defaults to /tty
      --version              Show current version
  -?, -h, --help             Show help information
```

[azure-ci-badge]: https://dev.azure.com/vandycknick/webtty/_apis/build/status/nickvdyck.webtty?branchName=master
[azure-ci-url]: https://dev.azure.com/vandycknick/webtty/_build/latest?definitionId=15&branchName=master

[nuget-package-url]: https://www.nuget.org/packages/webtty/
[nuget-package-badge]: https://img.shields.io/nuget/v/webtty.svg?style=flat-square&label=nuget

