import { ThunkAction } from "redux-thunk"
import { TerminalActions, AppState, UIActions } from "application/actions"
import { AppServices } from "application/interfaces"
import terminalSession from "application/actionCreators/terminalSession"
import selectTabs from "application/selectors/selectTabs"
import openNewTab from "./openNewTab"
import setTheme from "application/actionCreators/setTheme"

const startSession = (): ThunkAction<
    Promise<void>,
    AppState,
    AppServices,
    TerminalActions | UIActions
> => async (dispatch, getState, { connection, config }) => {
    dispatch(terminalSession("connecting"))
    dispatch(setTheme(config.theme))

    try {
        await connection.start()
        dispatch(terminalSession("connected"))
        const tabs = selectTabs(getState())

        if (tabs.length === 0) {
            dispatch(openNewTab())
        }
    } catch (ex) {
        dispatch(terminalSession("disconnected"))
    }
}

export default startSession
