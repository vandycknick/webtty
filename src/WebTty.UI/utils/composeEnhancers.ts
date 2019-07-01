import { compose } from "redux";

type ComposeEnhancer = typeof compose
type GlobalWindow = Window & { __REDUX_DEVTOOLS_EXTENSION_COMPOSE__?: ComposeEnhancer }

const global: GlobalWindow = window;

const composeEnhancers: ComposeEnhancer = (function composeEnhancersBuilder() {
    if (process.env.NODE_ENV == "production") return compose;

    return global.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ ?
        global.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ :
        compose
})()

export default composeEnhancers
