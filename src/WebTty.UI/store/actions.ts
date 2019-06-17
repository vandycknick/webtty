import msgpack5 from "msgpack5"
import { ThunkAction } from "redux-thunk"
import { AnyAction } from "redux";

import { TERMINAL_SESSION, TERMINAL_SESSION_DISCONNECTED, TERMINAL_NEW_TAB, AppState, AppConfig, TerminalActions } from "./types"
import fromEmitter, { $terminated } from "../utils/fromEmitter";
import { TerminalNewTabMessage, TerminalOutputMessage, TerminalInputMessage, TerminalResizeMessage } from "./models";
import MicroEmitter from "../utils/MicroEmitter";

let terminal: WebSocket | undefined;
let terminalEvenTarget = new MicroEmitter();

const terminalSessionStarted = (): TerminalActions => ({
    type: TERMINAL_SESSION
})

const terminalSessionDisconnected = (): TerminalActions => ({
    type: TERMINAL_SESSION_DISCONNECTED
})

const terminalNewTab = (msg: TerminalNewTabMessage): TerminalActions => ({
    type: TERMINAL_NEW_TAB,
    payload: { id: msg.id },
})

const startTerminal = (): ThunkAction<Promise<void>, AppState, AppConfig, AnyAction> =>
    async (dispatch) => {
        if (terminal) return

        terminal = await dispatch(connectToRemoteTerminal())

        const dataSource = fromEmitter<MessageEvent>(terminal)

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
                    const body = decoder.decode(properties[1])
                    const msg = new TerminalOutputMessage(body)
                    terminalEvenTarget.emit("message", msg)
                    break

                case 4:
                    const newTabMsg = new TerminalNewTabMessage(properties[1])
                    dispatch(terminalNewTab(newTabMsg))
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

async function writeToTerminal(msg: TerminalResizeMessage | TerminalInputMessage) {
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

const getTabStdoutStream = (_: number) =>
    (async function* () {
        const source = fromEmitter<TerminalOutputMessage>(terminalEvenTarget)
        for await (let message of source) {
            if (message === $terminated) break
            yield message.payload
        }
    })()

const writeStdin = (input: string) => {
    const msg = new TerminalInputMessage(input)
    writeToTerminal(msg)
}

const resizeTerminal = (cols: number, rows: number) => {
    const msg = new TerminalResizeMessage(cols, rows)
    writeToTerminal(msg)
}

export {
    startTerminal,
    getTabStdoutStream,
    writeStdin,
    resizeTerminal,
}
