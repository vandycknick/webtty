# 6. Messages library and JSON Schema

Date: 23/10/2019

## Status

Accepted

## Context
A well-defined communication layer is needed to send data back and forth between the backend and any connected clients.

## Decision
Any messages needed will be dined in a separate library (WebTty.Messages). This library will contain the message definitions and any utilities needed to work with those messages.

This library will be written in C# and will use dotnet annotations to define the contract, restrictions, ... for a certain message.

Extra build utilities will be provided to compile to clients written in a different language.

## Consequences
- An extra build step is needed to generate client code before it can be compiled.
