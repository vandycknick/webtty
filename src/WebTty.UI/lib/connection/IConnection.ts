import { IDisposable } from "lib/types"

interface IConnection extends IDisposable {
    start(): Promise<void>
    send(
        data: string | ArrayBuffer | SharedArrayBuffer | Blob | ArrayBufferView,
    ): void
    [Symbol.asyncIterator](): AsyncIterableIterator<string | ArrayBuffer>
}

export default IConnection
