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
    const message = new Message()
    message.init({
        Type: getCommandName(command),
        Payload: msgpack.encode(command.toJSON()).slice(),
    })

    return msgpack.encode([message.type, message.payload]).slice()
}

async function* deserializeMessages(dataStream: AsyncIterable<MessageEvent>): AsyncIterable<Events> {
    const msgpack = msgpack5()
    // const decoder = new TextDecoder()

    for await (let event of dataStream) {
        if (!event) continue

        const properties = msgpack.decode(Buffer.from(event.data))
        const message = new Message({ type: properties[0], payload: properties[1] })
        const data = msgpack.decode(Buffer.from(message.payload))

        switch (message.type) {
            case "TabOpened": {
                const command = new TabOpened()
                command.init(data)
                yield command
                break
            }

            case "TabResized": {
                const command = new TabResized()
                command.init(data)
                yield command
                break
            }

            case "StdOutStream": {
                const command = new StdOutStream()
                command.init(data)
                yield command
                break
            }

            case "StdErrStream": {
                const command = new StdErrorStream()
                command.init(data)
                yield command
                break
            }

            default:
                throw new Error("Unknown type")
        }
    }
}

export { Commands, Events }
export { serializeCommands, deserializeMessages }
