import { ThunkAction } from "redux-thunk"
import { TerminalActions, AppState } from "application/actions"
import { AppServices } from "application/interfaces"
import terminalSession from "application/actionCreators/terminalSession"
import selectTabs from "application/selectors/selectTabs"
import openNewTab from "./openNewTab"

const startSession = (): ThunkAction<
    Promise<void>,
    AppState,
    AppServices,
    TerminalActions
> => async (dispatch, getState, { connection }) => {
    dispatch(terminalSession("connecting"))

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
