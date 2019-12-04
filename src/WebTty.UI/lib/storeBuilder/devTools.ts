import { compose } from "redux"

type composeEnhancer = typeof compose

interface GlobalWindow {
    __REDUX_DEVTOOLS_EXTENSION_COMPOSE__: composeEnhancer
}

const global = (typeof window !== "undefined" ? window : {}) as GlobalWindow

const composeWithDevTool: composeEnhancer =
    typeof global !== "undefined" && global.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__
        ? global.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__
        : compose

export default composeWithDevTool
