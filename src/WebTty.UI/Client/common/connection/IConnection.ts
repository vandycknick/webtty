import { IDisposable } from "common/types"
enum ConnectionState {
    CONNECTING,
    OPEN,
    CLOSING,
    CLOSED,
}

interface IConnection
    extends IDisposable,
        AsyncIterableIterator<string | ArrayBuffer> {
    state: ConnectionState
    start(): Promise<void>
    send(
        data: string | ArrayBuffer | SharedArrayBuffer | Blob | ArrayBufferView,
    ): void
}

export { ConnectionState }
export default IConnection
