# 5. Library with platform native code for forking process with pty

Date: 23/10/2019

## Status

Accepted

## Context
Native code for different platforms will be needed to correctly start or fork a process and be able to send input and read outputs from this process.

## Decision
Any platform-specific code or code that needs to invoke into native code will be abstracted away into a separate library (WebTty.Native).
This library will expose a common API that does all the heavy lifting of detecting the current platform and calling into native API's.

The idea is to compile separate libraries for each platform and architecture. Those will get compiled and packed together into a single project. At runtime dotnet will load the correct dll for the current runtime and architecture.

Projects for each platform and architecture will use the following pattern `WebTty.Native.{platform}-{architecture}.csproj`. Examples:

- WebTty.Native.macos-x64.csproj
- WebTty.Native.windows-x64.csproj
- WebTty.Native.linux-x86.csrpoj


## Consequences
- The current setup might become a bit unwieldy when planning to target more platforms and architectures. Each runtime and platform will need a separate csproj file which includes all the required files.
