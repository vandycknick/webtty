import { produce } from "immer"
import { TerminalState, TerminalActions, TERMINAL_NEW_TAB_CREATED } from "./types"
import AsyncQueue from "../utils/AsyncQueue"
import { OpenNewTabCommand, ResizeTabCommand, SendInputCommand, StdOutStream, TabOpened } from "@webtty/messages"
import { Commands, Events } from "./serializers"

const initialState = {
    tabId: undefined,
}

type DispatchCommand = (command: Commands) => void
type Dispatch<A, R = void> = (action: A) => R

const terminalReducer = (state: TerminalState = initialState, action: TerminalActions): TerminalState =>
    produce(state, draft => {
        switch (action.type) {
            case TERMINAL_NEW_TAB_CREATED:
                draft.tabId = action.payload.id
        }
    })

const openNewTab = (dispatch: DispatchCommand) => (): void => {
    const command = new OpenNewTabCommand()
    dispatch(command)
}

const resizeTerminal = (dispatch: DispatchCommand) => (id: string, cols: number, rows: number): void => {
    const command = new ResizeTabCommand()
    command.init({
        TabId: id,
        Cols: cols,
        Rows: rows,
    })
    dispatch(command)
}

const writeStdIn = (dispatch: DispatchCommand) => (id: string, input: string): void => {
    const command = new SendInputCommand()
    command.init({
        TabId: id,
        Payload: input,
    })
    dispatch(command)
}

async function* stdoutMessageStream(id: string, messageStream: AsyncQueue<Events>): AsyncIterableIterator<string> {
    const decoder = new TextDecoder()
    for await (let message of messageStream) {
        if (message instanceof StdOutStream) {
            if (message.tabId == id) yield decoder.decode(Buffer.from(message.data))
        }
    }
}

const newTabMessageStream = (messageStream: AsyncQueue<Events>) =>
    async function(dispatch: Dispatch<TerminalActions>): Promise<void> {
        for await (let message of messageStream) {
            if (message instanceof TabOpened) {
                dispatch({ type: TERMINAL_NEW_TAB_CREATED, payload: { id: message.id } })
            }
        }
    }

export { terminalReducer }
export { openNewTab, resizeTerminal, writeStdIn, stdoutMessageStream, newTabMessageStream }
