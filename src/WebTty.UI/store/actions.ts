import msgpack5 from "msgpack5"
import { ThunkAction } from "redux-thunk"
import { AnyAction } from "redux";

import { TERMINAL_SESSION, TERMINAL_SESSION_DISCONNECTED, TERMINAL_NEW_TAB, AppState, AppConfig, TerminalActions, TERMINAL_NEW_TAB_CREATED } from "./types"
import fromEmitter, { $terminated } from "../utils/fromEmitter";
import { TerminalNewTabMessage, TerminalOutputMessage, TerminalInputMessage, TerminalResizeMessage, TerminalNewTabCreatedMessage } from "./models";
import MicroEmitter from "../utils/MicroEmitter";

let terminal: WebSocket | undefined;
let terminalEvenTarget = new MicroEmitter();

const terminalSessionStarted = (): TerminalActions => ({
    type: TERMINAL_SESSION
})

const terminalSessionDisconnected = (): TerminalActions => ({
    type: TERMINAL_SESSION_DISCONNECTED
})

const terminalNewTab = (): TerminalActions => ({
    type: TERMINAL_NEW_TAB,
})

const terminalNewTabCreated = (msg: TerminalNewTabCreatedMessage): TerminalActions => ({
    type: TERMINAL_NEW_TAB_CREATED,
    payload: { id: msg.id },
})

const startTerminal = (): ThunkAction<Promise<void>, AppState, AppConfig, AnyAction> =>
    async (dispatch) => {
        if (terminal) return

        terminal = await dispatch(connectToRemoteTerminal())

        const dataSource = fromEmitter<MessageEvent>(terminal)

        dispatch(createNewTab())

        const msgpack = msgpack5()
        const decoder = new TextDecoder()

        for await (let message of dataSource) {
            if (message === $terminated) break

            const properties = msgpack.decode(Buffer.from(message.data))
            const type: number = properties[0]

            switch (type) {
                case 0:
                case 1:
                case 3:
                    break

                case 2:
                    const id = properties[1]
                    const body = decoder.decode(properties[2])
                    const msg = new TerminalOutputMessage(id, body)
                    terminalEvenTarget.emit("message", msg)
                    break

                case 5:
                    const newTabMsg = new TerminalNewTabCreatedMessage(properties[1])
                    dispatch(terminalNewTabCreated(newTabMsg))
                    break

                default:
                    throw new Error("unknown type")
            }
        }

        terminalEvenTarget.emit("close", {});
        dispatch(terminalSessionDisconnected())
    }

const connectToRemoteTerminal = (): ThunkAction<Promise<WebSocket>, AppState, AppConfig, AnyAction> =>
    async (dispatch, _, config) => {
        const socket = new WebSocket(config.socketUrl)
        socket.binaryType = "arraybuffer"

        await isSocketReady(socket);

        dispatch(terminalSessionStarted())

        return socket;
    }

const createNewTab = (): ThunkAction<void, AppState, AppConfig, AnyAction> =>
    (dispatch) => {
        const msg = new TerminalNewTabMessage()
        writeToTerminal(msg)
        dispatch(terminalNewTab())
    }


async function writeToTerminal(msg: TerminalResizeMessage | TerminalInputMessage | TerminalNewTabMessage) {
    const msgpack = msgpack5()
    if (!terminal) return

    const payload = msgpack.encode(msg.serialize())
    terminal.send(payload.slice())
}

const isSocketReady = (socket: WebSocket) =>
    new Promise((res, rej) => {
        const isOpen = () => {
            socket.removeEventListener("open", isOpen)
            socket.removeEventListener("error", hasError)
            res()
        };
        const hasError = () => {
            socket.removeEventListener("open", isOpen)
            socket.removeEventListener("error", hasError)
            rej()
        }

        socket.addEventListener("open", isOpen)
        socket.addEventListener("error", hasError)
    })

const getTabStdoutStream = (id: number) =>
    (async function* () {
        const source = fromEmitter<TerminalOutputMessage>(terminalEvenTarget)
        for await (let message of source) {
            if (message === $terminated) break

            if (message.id == id) yield message.payload
        }
    })()

const writeStdin = (id: number, input: string) => {
    const msg = new TerminalInputMessage(id, input)
    writeToTerminal(msg)
}

const resizeTerminal = (id: number, cols: number, rows: number) => {
    const msg = new TerminalResizeMessage(id, cols, rows)
    writeToTerminal(msg)
}

export {
    startTerminal,
    getTabStdoutStream,
    createNewTab,
    writeStdin,
    resizeTerminal,
}
