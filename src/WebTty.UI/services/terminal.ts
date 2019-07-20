import { produce } from "immer"
import msgpack5 from "msgpack5"
import { TerminalState, TerminalActions, TERMINAL_NEW_TAB_CREATED } from "./types"
import AsyncQueue from "../utils/AsyncQueue"
import {
    TerminalNewTabMessage,
    TerminalResizeMessage,
    TerminalInputMessage,
    TerminalOutputMessage,
    TerminalNewTabCreatedMessage,
} from "./messages"

const initialState = {
    tabId: undefined,
}

const terminalReducer = (state: TerminalState = initialState, action: TerminalActions): TerminalState =>
    produce(state, draft => {
        switch (action.type) {
            case TERMINAL_NEW_TAB_CREATED:
                draft.tabId = action.payload.id
        }
    })

type Dispatch<A, R = void> = (action: A) => R

const openNewTab = (writeToSocket: (msg: TerminalNewTabMessage) => void) => (): void => {
    const msg = new TerminalNewTabMessage()
    writeToSocket(msg)
}

const resizeTerminal = (writeToSocket: (msg: TerminalResizeMessage) => void) => (
    id: number,
    cols: number,
    rows: number,
): void => {
    const msg = new TerminalResizeMessage(id, cols, rows)
    writeToSocket(msg)
}

const writeStdIn = (writeToSocket: (msg: TerminalInputMessage) => void) => (id: number, input: string): void => {
    const msg = new TerminalInputMessage(id, input)
    writeToSocket(msg)
}

async function* parseMessages(
    dataStream: AsyncIterable<MessageEvent>,
): AsyncIterable<TerminalOutputMessage | TerminalNewTabCreatedMessage> {
    const msgpack = msgpack5()
    const decoder = new TextDecoder()

    for await (let message of dataStream) {
        if (!message) continue

        const properties = msgpack.decode(Buffer.from(message.data))
        const type: number = properties[0]

        switch (type) {
            case 0:
            case 1:
            case 3:
                break

            case 2: {
                const id = properties[1]
                const body = decoder.decode(properties[2])
                const msg = new TerminalOutputMessage(id, body)
                yield msg
                break
            }
            case 5: {
                const msg = new TerminalNewTabCreatedMessage(properties[1])
                yield msg
                break
            }
            default:
                throw new Error("unknown type")
        }
    }
}

async function* stdoutMessageStream(
    id: number,
    messageStream: AsyncQueue<TerminalOutputMessage | TerminalNewTabCreatedMessage>,
): AsyncIterableIterator<string> {
    for await (let message of messageStream) {
        if (message instanceof TerminalOutputMessage) {
            if (message.id == id) yield message.payload
        }
    }
}

const newTabMessageStream = (messageStream: AsyncQueue<TerminalOutputMessage | TerminalNewTabCreatedMessage>) =>
    async function(dispatch: Dispatch<TerminalActions>): Promise<void> {
        for await (let message of messageStream) {
            if (message instanceof TerminalNewTabCreatedMessage) {
                dispatch({ type: TERMINAL_NEW_TAB_CREATED, payload: message })
            }
        }
    }

export { terminalReducer }
export { openNewTab, resizeTerminal, writeStdIn, stdoutMessageStream, parseMessages, newTabMessageStream }
