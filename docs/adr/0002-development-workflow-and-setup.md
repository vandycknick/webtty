# 2. Development workflow and setup

Date: 21/10/2019

## Status

Accepted

## Context

This repository could potentially contain multiple solutions that consist of multiple projects. For this, a build system is needed that is capable of creating multiple assemblies or executables. Some of those projects may require more advanced tooling because of the need to target multiple platforms or because of the requirement of a different dev stack (eg front end development). Thus it could be interesting to look at some build tools. Tools I considered using:
- Makefile
- Fake
- Cake
- Nuke
- Npm
- Yarn
- Webpack

## Decision

The idea is to keep things simple and easy to use by leveraging build tooling provided by dotnet. This means that `csproj` will be leveraged as much as possible. Any TypeScript related tools should be called via MSBuild targets. This makes sure that it's easy to get up and running via a CLI or an IDE.

For the front-end and TypeScript related projects, the decision is made to use `Yarn` to manage and install packages and `Webpack` to bundle any assets. Yarn installs and Webpack builds should be mainly called from MSBuild.

A general Makefile will be provided for Unix environments to ease CLI based development. This way many dotnet CLI commands can be orchestrated together. The plan is to only provide this for Unix based environments no such efforts will be made to add a higher level orchestration for Windows.

When extra commands are needed to bootstrap certain parts of the application those should then be thoroughly documented in the `README.md` file. We should try to avoid this as much as possible and when needed try to put measures in place to move away from this extra step.

## Consequences

- No Makefile equivalent for Windows means that CLI development could be more cumbersome because every command will need to be typed separately.
- Wrapping everything in MSBuild might require more effort, but makes it easy to develop via CLI or IDE's.
