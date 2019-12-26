import { decode } from "@msgpack/msgpack"

import { Messages } from "./types"
import BinaryMessageFormatter from "common/utils/BinaryMessageFormatter"
import { OpenNewTabReply, StdOutMessage } from "@webtty/messages"
import { UnknownMessage } from "./types"

function* deserializeMessages(
    buffer: ArrayBuffer | SharedArrayBuffer,
): IterableIterator<Messages> {
    const messages = BinaryMessageFormatter.parse(buffer)

    for (const message of messages) {
        yield deserializeMessage(message)
    }
}

const deserializeMessage = (buffer: Uint8Array): Messages => {
    const [type, payload] = decode(buffer) as [string, unknown]
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
            return new UnknownMessage(type, payload)
    }
}

export default deserializeMessages
