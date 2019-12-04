import { ThunkAction } from "redux-thunk"
import { StdInputRequest } from "@webtty/messages"

import { TerminalActions } from "../actions"
import { AppServices } from "../interfaces"

const writeInput = (
    tabId: string,
    payload: string,
): ThunkAction<void, null, AppServices, TerminalActions> => (
    _,
    __,
    { terminalManager },
) => {
    const message = new StdInputRequest({
        tabId,
        payload,
    })
    terminalManager.send(message)
}

export default writeInput
