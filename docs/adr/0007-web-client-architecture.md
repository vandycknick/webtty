# 8. Web Client Architecture

Date: 17/12/2019

## Status

Accepted

## Context
A web client is needed that can be easily and directly embedded into any backend system. This web client should be configurable and be able to set up a websocket connection with a webtty backend. After setting up a connection it should process messages and interpret commands/notifications from the backend and render accordingly.

## Decision
The web client will be a SPA application built in React using Redux to manage state in the front end. It will embed xterm.js in order to render a pty data stream and emulate a terminal. A simple webpack set up will build the application into a reasonable amount of chunks and takes care of cache invalidation by adding a hash to each bundled script file.

The web client will get bundled as a dll will export a razor page that can be routed to from any backend. Any assets will be included and packaged up into the application

## Consequences
- A separate set up is needed for development in order to benefit from hot module reloading.
