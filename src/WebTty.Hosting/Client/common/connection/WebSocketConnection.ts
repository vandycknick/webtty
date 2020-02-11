import AsyncQueue from "common/async/AsyncQueue"
import { IDisposable } from "common/types"
import IConnection, { ConnectionState } from "./IConnection"

interface WebSocketFactory {
    new (url: string, protocols?: string | string[]): WebSocket
}

class WebSocketConnection implements IConnection, IDisposable {
    private readonly queue = new AsyncQueue<MessageEvent>()
    private readonly url: string = ""
    private readonly binaryType: BinaryType
    private readonly WebSocketConstructor: WebSocketFactory
    private socket: WebSocket | undefined = undefined
    public state: ConnectionState = ConnectionState.CLOSED

    constructor(
        url: string,
        webSocketConstructor: WebSocketFactory,
        binaryType: BinaryType = "blob",
    ) {
        this.url = url
        this.WebSocketConstructor = webSocketConstructor
        this.binaryType = binaryType
    }

    public send(
        data: string | ArrayBuffer | SharedArrayBuffer | Blob | ArrayBufferView,
    ): void {
        this.socket?.send(data)
    }

    public start(): Promise<void> {
        const socket = new this.WebSocketConstructor(this.url)
        this.socket = socket
        this.state = ConnectionState.CONNECTING

        socket.binaryType = this.binaryType
        socket.onmessage = this.queue.enqueue

        return new Promise((res, rej) => {
            if (socket.readyState == socket.OPEN) {
                this.state = ConnectionState.OPEN
                socket.onerror = this.dispose
                socket.onclose = this.dispose
                res()
                return
            }

            const onOpen = (): void => {
                socket.onerror = this.dispose
                socket.onclose = this.dispose
                this.state = ConnectionState.OPEN
                res()
            }
            const onError = (err: CloseEvent): void => {
                this.dispose()
                rej(err)
            }
            socket.onopen = onOpen
            socket.onclose = onError
        })
    }

    public dispose = (): void => {
        this.queue.dispose()

        if (this.socket) {
            this.socket.close()
            this.socket.onmessage = null
            this.socket.onerror = null
            this.socket.onclose = null
        }

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

export { WebSocketFactory }
export default WebSocketConnection
