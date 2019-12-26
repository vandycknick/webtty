import { encode } from "@msgpack/msgpack"
import {
    OpenNewTabRequest,
    ResizeTabMessage,
    StdInputRequest,
    OpenNewTabReply,
    StdOutMessage,
} from "@webtty/messages"

import BinaryMessageFormatter from "common/utils/BinaryMessageFormatter"
import { Messages } from "./types"

const getName = (message: unknown): string => {
    if (message instanceof OpenNewTabRequest) return "OpenNewTabRequest"
    if (message instanceof OpenNewTabReply) return "OpenNewTabReply"
    if (message instanceof ResizeTabMessage) return "ResizeTabMessage"
    if (message instanceof StdInputRequest) return "StdInputRequest"
    if (message instanceof StdOutMessage) return "StdOutMessage"

    throw new Error("Unknown message!")
}

const serializeMessage = (message: Messages): ArrayBuffer => {
    const payload = encode([getName(message), message.toJSON()])
    return BinaryMessageFormatter.write(payload)
}

export default serializeMessage
