import { Action } from "redux"
import {
    OpenNewTabRequest,
    StdInputRequest,
    ResizeTabMessage,
} from "@webtty/messages"

const TERMINAL_SET_STATUS = "@webtty/TERMINAL_SET_STATUS"
const TERMINAL_SEND_MESSAGE = "@webtty/TERMINAL_SEND_MESSAGE"
const TERMINAL_TAB_CREATED = "@webtty/TERMINAL_NEW_TAB"

type SetStatusAction = Action<typeof TERMINAL_SET_STATUS> & {
    payload: "connecting" | "connected"
}

type SendMessageAction = Action<typeof TERMINAL_SEND_MESSAGE> & {
    payload: OpenNewTabRequest | StdInputRequest | ResizeTabMessage
}

type TabCreatedAction = Action<typeof TERMINAL_TAB_CREATED> & {
    payload: {
        id: string
    }
}

type TerminalActions = SetStatusAction | SendMessageAction | TabCreatedAction

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

const openNewTab = (): SendMessageAction => {
    const tab = new OpenNewTabRequest()
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
    const message = new ResizeTabMessage({
        tabId,
        cols,
        rows,
    })
    return sendMessage(message)
}

const writeStdIn = (tabId: string, message: string): SendMessageAction => {
    const input = new StdInputRequest({
        tabId,
        payload: message,
    })
    return sendMessage(input)
}

export { TERMINAL_SET_STATUS, TERMINAL_SEND_MESSAGE, TERMINAL_TAB_CREATED }
export { TerminalActions }
export { setStatus, sendMessage, openNewTab, newTab, resizeTab, writeStdIn }
