import { useState, useRef, useCallback, useMemo } from "preact/hooks"
import fromEmitter from "../utils/fromEmitter"
import useEventListener from "./useEventListener"

type SendMessage = (data: string | ArrayBufferLike | Blob | ArrayBufferView) => void
type WebSocketMessageType = string | ArrayBuffer | SharedArrayBuffer | Blob | ArrayBufferView

const useWebSocket = (
    url: string,
    options: { binaryType?: BinaryType },
): [AsyncIterable<MessageEvent>, SendMessage, () => void] => {
    const messageBuffer = useRef<WebSocketMessageType[]>([])
    const [retry, setRetry] = useState(10)

    const [socket, dataStream] = useMemo(() => {
        const socket = new WebSocket(url)
        if (options.binaryType) socket.binaryType = options.binaryType

        const dataStream = fromEmitter<MessageEvent>(socket)
        return [socket, dataStream]
    }, [url])

    const sendMessage = useCallback(
        (message: string | ArrayBuffer | SharedArrayBuffer | Blob | ArrayBufferView) => {
            if (socket.readyState !== WebSocket.OPEN) {
                messageBuffer.current.push(message)
            } else {
                socket.send(message)
            }
        },
        [socket],
    )

    const drainQueue = useCallback(() => {
        let message
        while ((message = messageBuffer.current.pop()) != undefined) {
            socket.send(message)
        }
    }, [socket])

    const retryConnection = useCallback(() => setRetry(retry - 1), [])

    useEventListener("open", drainQueue, socket)

    return [dataStream, sendMessage, retryConnection]
}

export default useWebSocket
