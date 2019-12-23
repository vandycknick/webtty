import { IDisposable } from "common/types"

interface IConnection extends IDisposable, AsyncIterable<string | ArrayBuffer> {
    start(): Promise<void>
    send(
        data: string | ArrayBuffer | SharedArrayBuffer | Blob | ArrayBufferView,
    ): void
}

export default IConnection
