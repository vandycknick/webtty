import { ThunkAction } from "redux-thunk"
import { ResizeTabMessage } from "@webtty/messages"

import { TerminalActions } from "../actions"
import { AppServices } from "../interfaces"

const resizeTab = (
    tabId: string,
    cols: number,
    rows: number,
): ThunkAction<void, null, AppServices, TerminalActions> => (
    _,
    __,
    { terminalManager },
) => {
    const message = new ResizeTabMessage({
        tabId,
        cols,
        rows,
    })
    terminalManager.send(message)
}

export default resizeTab
