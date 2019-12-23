import AsyncQueue from "common/AsyncQueue"
import { IDisposable } from "common/types"
import IConnection from "./IConnection"

class WebSocketConnection implements IConnection, IDisposable {
    private queue = new AsyncQueue<MessageEvent>()
    private socket: WebSocket | undefined = undefined
    private url = ""
    private binaryType: BinaryType | undefined = undefined

    constructor(url: string, binaryType?: BinaryType) {
        this.url = url
        this.binaryType = binaryType
    }

    public send(
        data: string | ArrayBuffer | SharedArrayBuffer | Blob | ArrayBufferView,
    ): void {
        this.socket?.send(data)
    }

    public start(): Promise<void> {
        this.socket = new WebSocket(this.url)

        if (this.binaryType !== undefined) {
            this.socket.binaryType = this.binaryType
        }

        this.socket.addEventListener("message", this.queue.push)
        this.socket.addEventListener("error", this.errorListener)
        this.socket.addEventListener("close", this.dispose)

        return new Promise((res, rej) => {
            if (this.socket === undefined) {
                rej(new Error("Unknown error, socket is undefined"))
            }

            if (this.socket?.readyState == this.socket?.OPEN) {
                res()
                return
            }

            const remove = (): void => {
                this.socket?.removeEventListener("open", onOpen)
                this.socket?.removeEventListener("close", onError)
            }
            const onOpen = (): void => {
                res()
                remove()
            }
            const onError = (err: CloseEvent): void => {
                rej(err)
                remove
            }
            this.socket?.addEventListener("open", onOpen)
            this.socket?.addEventListener("close", onError)
        })
    }

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    private errorListener(...args: any[]): void {
        this.queue.throw(args[0])
        this.dispose()
    }

    public dispose(): void {
        this.queue.dispose()
        this.socket?.close()
        this.socket?.removeEventListener("message", this.queue.push)
        this.socket?.removeEventListener("error", this.errorListener)
        this.socket?.removeEventListener("close", this.dispose)

        this.socket = undefined
    }

    public async *[Symbol.asyncIterator](): AsyncIterableIterator<string> {
        for await (const output of this.queue) {
            yield output.data
        }
    }
}

export default WebSocketConnection
