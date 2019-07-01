import { createStore, combineReducers, applyMiddleware } from "redux"
import thunk from "redux-thunk"

import { terminal } from "./reducers"
import configBuilder from "./config"
import composeEnhancers from "../utils/composeEnhancers"

import { AppStore, AppState, AppConfig } from "./types"

const storeBuilder = (): AppStore => {
    const config = configBuilder()
    const reducers = combineReducers<AppState>({ terminal })
    const middleware = thunk.withExtraArgument<AppConfig>(config)

    const store = createStore(
        reducers,
        composeEnhancers(applyMiddleware(middleware))
    );

    return store
}

export { startTerminal, getTabStdoutStream, writeStdin, resizeTerminal } from "./actions"
export { AppState }
export { storeBuilder }
