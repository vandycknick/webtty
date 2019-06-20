import { createStore, combineReducers, applyMiddleware } from "redux"
import thunk from "redux-thunk"

import { terminal } from "./reducers"
import { AppState, AppConfig } from "./types"

const reducers = combineReducers<AppState>({ terminal })
const middleware = thunk.withExtraArgument({
    socketUrl: "ws://localhost:5000/ws"
} as AppConfig)

const storeBuilder = () =>
    createStore(
        reducers,
        applyMiddleware(middleware)
    );

export { startTerminal, getTabStdoutStream, writeStdin, resizeTerminal } from "./actions"
export { AppState }
export { storeBuilder }
