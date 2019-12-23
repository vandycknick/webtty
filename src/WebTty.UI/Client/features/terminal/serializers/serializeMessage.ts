import { encode } from "@msgpack/msgpack"
import {
    OpenNewTabRequest,
    ResizeTabMessage,
    StdInputRequest,
} from "@webtty/messages"

import BinaryMessageFormatter from "common/BinaryMessageFormatter"
import Messages from "./Messages"

const getName = (message: unknown): string => {
    if (message instanceof OpenNewTabRequest) return "OpenNewTabRequest"
    if (message instanceof ResizeTabMessage) return "ResizeTabMessage"
    if (message instanceof StdInputRequest) return "StdInputRequest"

    return ""
}

const serializeMessage = (message: Messages): ArrayBuffer => {
    const payload = encode([getName(message), message.toJSON()]).slice()
    const data = BinaryMessageFormatter.write(payload)
    return data
}

export default serializeMessage
