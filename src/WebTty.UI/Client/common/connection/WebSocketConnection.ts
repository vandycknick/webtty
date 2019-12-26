import AsyncQueue from "common/async/AsyncQueue"
import { IDisposable } from "common/types"
import IConnection, { ConnectionState } from "./IConnection"

class WebSocketConnection implements IConnection, IDisposable {
    private readonly queue = new AsyncQueue<MessageEvent>()
    private socket: WebSocket | undefined = undefined
    private url = ""
    private binaryType: BinaryType | undefined = undefined
    public state: ConnectionState = ConnectionState.CLOSED

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
        this.state = ConnectionState.CONNECTING

        if (this.binaryType !== undefined) {
            this.socket.binaryType = this.binaryType
        }

        this.socket.addEventListener("message", this.queue.enqueue)
        this.socket.addEventListener("error", this.dispose)
        this.socket.addEventListener("close", this.dispose)

        return new Promise((res, rej) => {
            if (this.socket === undefined) {
                rej(new Error("Unknown error, socket is undefined"))
            }

            if (this.socket?.readyState == this.socket?.OPEN) {
                this.state = ConnectionState.OPEN
                res()
                return
            }

            const remove = (): void => {
                this.socket?.removeEventListener("open", onOpen)
                this.socket?.removeEventListener("close", onError)
            }
            const onOpen = (): void => {
                this.state = ConnectionState.OPEN
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

    public dispose = (): void => {
        this.queue?.dispose()
        this.socket?.close()
        this.socket?.removeEventListener("message", this.queue.enqueue)
        this.socket?.removeEventListener("error", this.dispose)
        this.socket?.removeEventListener("close", this.dispose)

        this.socket = undefined
        this.state = ConnectionState.CLOSED
    }

    public async next(): Promise<
        IteratorResult<string | ArrayBuffer, undefined>
    > {
        const result = await this.queue.next()

        return {
            ...result,
            value: result.value?.data,
        }
    }

    public [Symbol.asyncIterator] = (): AsyncIterableIterator<
        string | ArrayBuffer
    > => this
}

export default WebSocketConnection
