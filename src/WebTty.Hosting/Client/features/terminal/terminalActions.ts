import { Action, AnyAction } from "redux"
import {
    OpenNewTabRequest,
    ResizeTabRequest,
    SendInputRequest,
    OpenOutputRequest,
} from "@webtty/messages"
import { v4 as uuidv4 } from "uuid"

const TERMINAL_SET_STATUS = "@webtty/TERMINAL_SET_STATUS"
const TERMINAL_SEND_MESSAGE = "@webtty/TERMINAL_SEND_MESSAGE"
const TERMINAL_TAB_CREATED = "@webtty/TERMINAL_NEW_TAB"

type SetStatusAction = Action<typeof TERMINAL_SET_STATUS> & {
    payload: "connecting" | "connected"
}

type SendMessageAction = Action<typeof TERMINAL_SEND_MESSAGE> & {
    payload:
        | OpenNewTabRequest
        | ResizeTabRequest
        | SendInputRequest
        | OpenOutputRequest
}

type TabCreatedAction = Action<typeof TERMINAL_TAB_CREATED> & {
    payload: {
        id: string
    }
}

type TerminalActions =
    | SetStatusAction
    | SendMessageAction
    | TabCreatedAction
    | AnyAction

const setStatus = (status: SetStatusAction["payload"]): SetStatusAction => ({
    type: TERMINAL_SET_STATUS,
    payload: status,
})

const sendMessage = (
    message: SendMessageAction["payload"],
): SendMessageAction => ({
    type: TERMINAL_SEND_MESSAGE,
    payload: message,
})

const openNewTab = (id: string = uuidv4()): SendMessageAction => {
    const tab = new OpenNewTabRequest({ id, title: "" })
    return sendMessage(tab)
}

const newTab = (id: string): TabCreatedAction => ({
    type: TERMINAL_TAB_CREATED,
    payload: {
        id,
    },
})

const resizeTab = (
    tabId: string,
    cols: number,
    rows: number,
): SendMessageAction => {
    const request = new ResizeTabRequest({
        tabId,
        cols,
        rows,
    })
    return sendMessage(request)
}

const openStdout = (id: string): SendMessageAction => {
    const request = new OpenOutputRequest({
        tabId: id,
    })
    return sendMessage(request)
}

const writeStdIn = (tabId: string, message: string): SendMessageAction => {
    const input = new SendInputRequest({
        tabId,
        payload: message,
    })
    return sendMessage(input)
}

export { TERMINAL_SET_STATUS, TERMINAL_SEND_MESSAGE, TERMINAL_TAB_CREATED }
export { TerminalActions }
export {
    setStatus,
    sendMessage,
    openNewTab,
    newTab,
    resizeTab,
    writeStdIn,
    openStdout,
}
