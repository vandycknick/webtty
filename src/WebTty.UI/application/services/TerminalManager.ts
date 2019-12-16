import { encode, decode } from "@msgpack/msgpack"
import {
    OpenNewTabReply,
    StdOutMessage,
    OpenNewTabRequest,
    StdInputRequest,
    ResizeTabMessage,
} from "@webtty/messages"
import IConnection from "lib/connection/IConnection"
import BinaryMessageFormat from "lib/utils/BinaryMessageFormat"
import { MemoryStream } from "lib/io"

type Messages =
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

const serializeCommand = (message: Messages): ArrayBuffer => {
    const bytes = encode(message.toJSON())
    const payload = encode([getName(message), bytes]).slice()
    const data = BinaryMessageFormat.write(payload)
    return data
}

class TerminalManager {
    private readonly connection: IConnection
    private readonly stdoutMap: Map<string, MemoryStream> = new Map()

    constructor(connection: IConnection) {
        this.connection = connection
    }

    public write(tabId: string, payload: string): void {
        if (!this.stdoutMap.has(tabId)) {
            this.stdoutMap.set(tabId, new MemoryStream())
        }
        this.stdoutMap.get(tabId)?.write(payload)
    }

    public getStdout(tabId: string): AsyncIterableIterator<string> {
        const stream = this.stdoutMap.get(tabId)

        if (stream == undefined) {
            const newStream = new MemoryStream()
            this.stdoutMap.set(tabId, newStream)
            return newStream[Symbol.asyncIterator]()
        }

        return stream[Symbol.asyncIterator]()
    }

    public send(
        msg: OpenNewTabRequest | StdInputRequest | ResizeTabMessage,
    ): void {
        const data = serializeCommand(msg)
        this.connection.send(data)
    }

    public async *messages(): AsyncIterableIterator<
        OpenNewTabReply | StdOutMessage
    > {
        for await (const data of this.connection) {
            if (!data) continue
            // TODO: maybe throw an error??
            if (typeof data === "string") continue

            const messages = BinaryMessageFormat.parse(data)

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
                        stdOut.data = Array.from(
                            (payload as { Data: number[] })["Data"],
                        )
                        yield stdOut
                        break
                    }

                    default:
                        continue
                }
            }
        }
    }
}

export default TerminalManager
