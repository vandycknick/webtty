import { produce } from "immer"
import { TerminalState, TerminalActions, TERMINAL_NEW_TAB_CREATED } from "./types"
import AsyncQueue from "../utils/AsyncQueue"
import { OpenNewTabRequest, OpenNewTabReply, StdOutMessage, ResizeTabMessage, StdInputRequest } from "@webtty/messages"
import { Messages } from "./serializers"

const initialState = {
    tabId: undefined,
}

type DispatchCommand = (command: Messages) => void
type Dispatch<A, R = void> = (action: A) => R

const terminalReducer = (state: TerminalState = initialState, action: TerminalActions): TerminalState =>
    produce(state, draft => {
        switch (action.type) {
            case TERMINAL_NEW_TAB_CREATED:
                draft.tabId = action.payload.id
        }
    })

const openNewTab = (dispatch: DispatchCommand) => (): void => {
    const command = new OpenNewTabRequest()
    dispatch(command)
}

const resizeTerminal = (dispatch: DispatchCommand) => (id: string, cols: number, rows: number): void => {
    const resize = new ResizeTabMessage({ tabId: id, cols, rows })
    dispatch(resize)
}

const writeStdIn = (dispatch: DispatchCommand) => (id: string, payload: string): void => {
    const input = new StdInputRequest({ tabId: id, payload })
    dispatch(input)
}

async function* stdoutMessageStream(id: string, messageStream: AsyncQueue<Messages>): AsyncIterableIterator<string> {
    const decoder = new TextDecoder()
    for await (const message of messageStream) {
        if (message instanceof StdOutMessage) {
            if (message.tabId == id) yield decoder.decode(Buffer.from(message.data))
        }
    }
}

const newTabMessageStream = (messageStream: AsyncQueue<Messages>) =>
    async function(dispatch: Dispatch<TerminalActions>): Promise<void> {
        for await (const message of messageStream) {
            if (message instanceof OpenNewTabReply) {
                dispatch({ type: TERMINAL_NEW_TAB_CREATED, payload: { id: message.id } })
            }
        }
    }

export { terminalReducer }
export { openNewTab, resizeTerminal, writeStdIn, stdoutMessageStream, newTabMessageStream }
