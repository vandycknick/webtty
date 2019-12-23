import { decode } from "@msgpack/msgpack"

import Messages from "./Messages"
import BinaryMessageFormatter from "common/BinaryMessageFormatter"
import { OpenNewTabReply, StdOutMessage } from "@webtty/messages"

function* deserializeMessages(
    buffer: ArrayBuffer,
): IterableIterator<Messages | undefined> {
    const messages = BinaryMessageFormatter.parse(buffer)

    for (const message of messages) {
        yield deserializeMessage(message)
    }
}

const deserializeMessage = (buffer: Uint8Array): Messages | undefined => {
    const decoded = decode(buffer) as [string, Buffer]
    const type = decoded[0]
    const payload = decode(decoded[1])
    switch (type) {
        case "OpenNewTabReply": {
            return OpenNewTabReply.fromJS(payload)
        }

        case "StdOutMessage": {
            const message = StdOutMessage.fromJS(payload)
            message.data = Array.from((payload as { Data: number[] })["Data"])
            return message
        }

        default:
            return undefined
    }
}

export { deserializeMessages }
