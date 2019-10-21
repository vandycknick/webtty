import { encode, decode } from "@msgpack/msgpack"
import { OpenNewTabReply, StdOutMessage, OpenNewTabRequest, StdInputRequest, ResizeTabMessage } from "@webtty/messages"
import { BinaryMessageFormat } from "utils/BinaryFormat"

type Messages =
    | OpenNewTabReply
    | StdOutMessage
    | OpenNewTabRequest
    | OpenNewTabReply
    | ResizeTabMessage
    | StdInputRequest

const getName = (message: unknown): string => {
    if (message instanceof OpenNewTabRequest) return "OpenNewTabRequest"
    if (message instanceof ResizeTabMessage) return "ResizeTabMessage"
    if (message instanceof StdInputRequest) return "StdInputRequest"

    return ""
}

const serializeCommands = (message: Messages): ArrayBuffer => {
    const bytes = encode(message.toJSON())
    const payload = encode([getName(message), bytes]).slice()
    const data = BinaryMessageFormat.write(payload)
    return data
}

async function* deserializeMessages(dataStream: AsyncIterable<MessageEvent>): AsyncIterable<Messages> {
    for await (const event of dataStream) {
        if (!event) continue

        const messages = BinaryMessageFormat.parse(event.data)

        for (const message of messages) {
            const decoded = decode(message) as [string, Buffer]
            const type = decoded[0]
            const payload = decode(decoded[1])

            switch (type) {
                case "OpenNewTabReply":
                    yield OpenNewTabReply.fromJS(payload)
                    break

                case "StdOutMessage": {
                    const stdOut = StdOutMessage.fromJS(payload)
                    stdOut.data = Array.from((payload as { Data: number[] })["Data"])
                    yield stdOut
                    break
                }

                default:
                    continue
            }
        }
    }
}

export { Messages }
export { serializeCommands, deserializeMessages }
