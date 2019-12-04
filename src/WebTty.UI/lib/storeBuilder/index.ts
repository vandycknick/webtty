import {
    Action,
    Reducer,
    Middleware,
    Store,
    createStore,
    applyMiddleware,
    compose,
    combineReducers,
    ReducersMapObject,
} from "redux"
import thunk from "redux-thunk"

import composeEnhancer from "./devTools"

class StoreBuilder<S, A extends Action> {
    private middleware: Middleware[] = []
    private rootReducer: Reducer<S, A> | undefined = undefined
    private composeEnhancer = compose

    useThunkMiddleware<TServices>(services: TServices): this {
        this.middleware.push(thunk.withExtraArgument(services))
        return this
    }

    useDevTools(enabled = false): this {
        if (enabled) {
            this.composeEnhancer = composeEnhancer
        }
        return this
    }

    useReducer(reducerMap: ReducersMapObject<S, A>): this {
        this.rootReducer = combineReducers(reducerMap)
        return this
    }

    build(): Store<S, A> {
        if (this.rootReducer === undefined) {
            throw new Error("At least one reducer should be defined")
        }

        const enhancer = this.composeEnhancer(
            applyMiddleware(...this.middleware),
        )
        return createStore(this.rootReducer, enhancer)
    }
}

export default StoreBuilder
