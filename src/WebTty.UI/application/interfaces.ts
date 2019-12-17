import { Action } from "redux"
import { ThunkAction } from "redux-thunk"

import IConnection from "lib/connection/IConnection"
import TerminalManager from "./services/TerminalManager"
import { TerminalActions, UIActions } from "./actions"

interface AppConfig {
    ttyHost: string
    ttyPath: string
    theme: string
}

interface AppServices {
    terminalManager: TerminalManager
    connection: IConnection
    config: AppConfig
}

interface Dispatch<TBasicAction extends Action> {
    <TReturnType, TState, TExtraThunkArg>(
        thunkAction: ThunkAction<
            TReturnType,
            TState,
            TExtraThunkArg,
            TBasicAction
        >,
    ): TReturnType
}

type AppDispatch = Dispatch<TerminalActions | UIActions>

export { AppServices, AppConfig, AppDispatch }
