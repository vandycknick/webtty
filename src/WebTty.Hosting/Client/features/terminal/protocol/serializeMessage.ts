import { encode } from "@msgpack/msgpack"
import {
    OpenNewTabRequest,
    OpenNewTabReply,
    ResizeTabRequest,
    SendInputRequest,
    OpenOutputRequest,
    OutputEvent,
} from "@webtty/messages"

import BinaryMessageFormatter from "common/utils/BinaryMessageFormatter"
import { Messages } from "./types"

const getName = (message: unknown): string => {
    if (message instanceof OpenNewTabRequest) return "OpenNewTabRequest"
    if (message instanceof OpenNewTabReply) return "OpenNewTabReply"
    if (message instanceof ResizeTabRequest) return "ResizeTabRequest"
    if (message instanceof SendInputRequest) return "SendInputRequest"
    if (message instanceof OpenOutputRequest) return "OpenOutputRequest"
    if (message instanceof OutputEvent) return "OutputEvent"

    throw new Error("Unknown message!")
}

const serializeMessage = (message: Messages): ArrayBuffer => {
    const payload = encode([getName(message), message.toJSON()])
    return BinaryMessageFormatter.write(payload)
}

export default serializeMessage
