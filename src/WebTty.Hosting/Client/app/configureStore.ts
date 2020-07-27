import {
    createStore,
    applyMiddleware,
    Store,
    StoreEnhancer,
    Middleware,
} from "redux"
import { composeWithDevTools } from "redux-devtools-extension/developmentOnly"
import thunk from "redux-thunk"

import rootReducer, { RootState, RootAction } from "app/rootReducer"

interface ConfigureStoreOptions {
    middleware: Middleware[]
    state?: Partial<RootState>
}

const configureStore = (
    options: ConfigureStoreOptions = { middleware: [] },
): Store<RootState, RootAction> => {
    const middleware = [...options.middleware, thunk]
    const middlewareEnhancer = applyMiddleware(...middleware)

    const storeEnhancers: StoreEnhancer[] = [middlewareEnhancer]
    const composedEnhancer = composeWithDevTools(...storeEnhancers)

    const store = createStore<RootState, RootAction, unknown, unknown>(
        rootReducer,
        options.state,
        composedEnhancer,
    )

    if (process.env.NODE_ENV !== "production") {
        module.hot?.accept("app/reducers/rootReducer", async () => {
            // eslint-disable-next-line @typescript-eslint/no-var-requires
            const newRootReducer = require("app/rootReducer").default
            store.replaceReducer(newRootReducer)
        })
    }

    return store
}

export default configureStore
