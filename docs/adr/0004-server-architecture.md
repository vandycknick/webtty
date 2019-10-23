# 4. Server architecture

Date: 21/10/2019

## Status

Accepted

## Context

The application should spin up a web server that accept incoming connections and allow a user to turn CLI tools into web applications.

## Decision

ASP.NET will be used as the framework of choice in order to do all the low level http plumbing and provide a nice abstraction to consume a websocket. The tool will be a cli application and the server will be included inside that project.

A custom middleware layer will be created to easily add custom logic into the http request pipeline and accept incoming connections.

The application will contain the following layers

1. Transport

This is the initial layer that will accept incoming connections and takes care of routing messages, frames, events, ... into the application. Examples of this are a WebSocketTransport, Long polling Transport, Server sent events, ... This allows for easily broaden the support range of this application without affecting other layers of the architecture

2. Protocol

This layer deserializes incoming bytes into the correct format defined by the protocol. (eg Binary, json, xml, ...)

3. Connection

This is the glue layer between transport and handlers. It will take any incoming message and call its appropriate handler.

4. Handlers

This contains any business logic related to an incoming messages.

```
transport -> protocol -> connection -> handlers
```

## Consequences
- Given this architecture it should be possible to easily extend and broaden the supported browser set. (Long polling, server sent events, ...)
- At the moment websockets is the only supported transport layer. Any handshaking logic to determine the supported transport layer will be defined when/if needed in a future ADR.
