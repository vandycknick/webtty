import { ThunkAction } from "redux-thunk"
import { OpenNewTabRequest } from "@webtty/messages"

import { TerminalActions, AppState } from "../actions"
import { AppServices } from "../interfaces"
import terminalNewTab from "../actionCreators/terminalNewTab"

const openNewTab = (): ThunkAction<
    void,
    AppState,
    AppServices,
    TerminalActions
> => (dispatch, _, { terminalManager }) => {
    const msg = new OpenNewTabRequest()
    terminalManager.send(msg)
    dispatch(terminalNewTab())
}

export default openNewTab
