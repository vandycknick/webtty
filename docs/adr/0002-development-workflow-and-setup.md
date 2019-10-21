# 2. Development workflow and setup

Date: 21/10/2019

## Status

Accepted

## Context

This repository could potentially contain multiple solutions that all consists of multiple projects. For this I will need a build system that is capable of creating multiple assemblies or executables. Some of those projects may require more advanced tooling because of the need to target multiple platforms or because to the requirement of a different dev stack (eg front end development). Thus I need some extra tooling to make development easier and make it possible to publish a single artefact.

The different tools I considered for this:
- Makefile
- Fake
- Cake
- Nuke
- Npm
- Yarn
- Webpack

## Decision

I will use standard MSBuild and csproj files to create projects that produce .NET assemblies and executables. For front-end development I will use yarn and webpack in order to bundle front-end code into a single artefact.

To abstract away msbuild, dotnet cli, yarn, ... I will use Nuke. This allows me to use the same workflow on different operating systems. It also allows me to have a single command line interface to lint, test, build and package this whole project. And as an added benefit I can leverage C# to write my build scripts and use rich IDE integrations (intellisense, ...).

My main focus will be around tooling geared towards a cli first and VSCode workflow.

The main high level commands provided:

- `nuke setup` Will install all dependencies and generate all code required needed to work on this project.
- `nuke check` Will run all checks over the code base (linting, unit, e2e, ...).
- `nuke build` Will produce an executable.
- `nuke package` Will create a shareable artefact.

## Consequences

- It will not be possible to just double click the solution file and get started in Visual Studio. In order to run the solution in Visual Studio some setup process will need to be done first before the project can be opened in Visual Studio. Documentation for this will need to be added to or linked from the readme.
- Nuke will require some extra knowledge in order to contribute to the build system. But given the low frequency of changes to this I think this is acceptable.
