import { TerminalMessage, TerminalInputMessage, TerminalResizeMessage, TerminalNewTabMessage } from "./messages"
import msgpack5 from "msgpack5"

const msgpack = msgpack5()

const serializeTerminalMessage = (msg: TerminalMessage): Buffer => {
    if (msg instanceof TerminalInputMessage) {
        const packed = [msg.type, msg.id, msg.payload]
        const payload = msgpack.encode(packed)
        return payload.slice()
    } else if (msg instanceof TerminalResizeMessage) {
        const packed = [msg.type, msg.id, msg.cols, msg.rows]
        const payload = msgpack.encode(packed)
        return payload.slice()
    } else if (msg instanceof TerminalNewTabMessage) {
        const packed = [msg.type]
        const payload = msgpack.encode(packed)
        return payload.slice()
    }

    throw new Error("Unknown message")
}

export { serializeTerminalMessage }
