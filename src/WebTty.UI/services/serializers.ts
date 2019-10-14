import msgpack5 from "msgpack5"

import {
    TabOpened,
    Message,
    TabResized,
    StdOutStream,
    StdErrorStream,
    OpenNewTabCommand,
    ResizeTabCommand,
    SendInputCommand,
} from "@webtty/messages"

type Commands = OpenNewTabCommand | ResizeTabCommand | SendInputCommand
type Events = TabOpened | TabResized | StdOutStream | StdErrorStream

const getCommandName = (command: Commands): string => {
    if (command instanceof OpenNewTabCommand) return "OpenNewTabCommand"

    if (command instanceof ResizeTabCommand) return "ResizeTabCommand"

    if (command instanceof SendInputCommand) return "SendInputCommand"

    throw new Error("Unknown command")
}

const serializeCommands = (command: Commands): Buffer => {
    const msgpack = msgpack5()
    const message = new Message({
        type: getCommandName(command),
        payload: msgpack.encode(command.toJSON()) as any,
    })

    return msgpack.encode([message.type, message.payload]).slice()
}

async function* deserializeMessages(dataStream: AsyncIterable<MessageEvent>): AsyncIterable<Events> {
    const msgpack = msgpack5()

    for await (const event of dataStream) {
        if (!event) continue

        const messages: Buffer[] = []
        let buffer = Buffer.from(event.data)
        while (buffer.length) {
            const index = buffer.findIndex((_, i, b) => {
                if (b[i + 1] === 163 && b[i + 2] === 101 && b[i + 3] === 110 && b[i + 4] === 100 && b[i + 5] === 147) {
                    return true
                }
                return false
            })
            if (index > 1) {
                messages.push(buffer.slice(0, index + 5))
                buffer = buffer.slice(index + 5)
            } else {
                messages.push(buffer)
                buffer = Buffer.from([])
            }
        }

        for (const part of messages) {
            const properties = msgpack.decode(part)
            const message = new Message({ type: properties[0], payload: properties[1] })
            const data = properties[1]

            switch (message.type) {
                case "TabOpened": {
                    const command = TabOpened.fromJS(data)
                    yield command
                    break
                }

                case "TabResized": {
                    const command = TabResized.fromJS(data)
                    yield command
                    break
                }

                case "StdOutStream": {
                    const command = StdOutStream.fromJS(data)
                    command.data = Array.from(data["Data"])
                    yield command
                    break
                }

                case "StdErrStream": {
                    const command = StdErrorStream.fromJS(data)
                    command.data = Array.from(data["Data"])
                    yield command
                    break
                }

                default:
                    throw new Error("Unknown type")
            }
        }
    }
}

export { Commands, Events }
export { serializeCommands, deserializeMessages }
