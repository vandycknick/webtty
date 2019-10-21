# 4. Server architecture

Date: 21/10/2019

## Status

Accepted

## Context

The application should spin up a web server that accept incoming connections and allows CLI tools to turn into web applications.

## Decision

ASP.NET will be used as the framework of choice in order to do all the low level http and websocket handling. The tool will be a cli application and the server will be included inside that project.

A custom middleware layer will be created to easily add custom logic into the http request pipeline and accept incoming connections.

The application will contain the following layers

1. Transport
This is the initial layer that will accept incoming connections and takes care of routing messages, frames, events, ... into the application. Examples of this are a WebSocketTransport, Long polling Transport, Server sent events, ... This allows for easily broaden the support range of this application without affecting other layers of the architecture

2. Protocol
This layer deserializes incoming bytes into the correct format defined by the protocol. (eg Binary, json, xml, ...)

3. Connection
This is the glue layer between transport and handlers. It will take any incoming deserialized message and call its appropriate handler.

4. Handlers
This is where the core business logic resides.

```
transport -> protocol -> connection -> handlers
```

## Consequences
- This layer makes it possible to easily extend the application.
- It possible to support a broad range of browser because of the possibility to feature detect and fall back to server sent events or long polling if necessary.
