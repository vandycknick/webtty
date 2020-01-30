import WebSocketConnection from "./WebSocketConnection"
import { ConnectionState } from "./IConnection"

describe("WebSocketConnection", () => {
    const fakeURL = "ws://nothere"

    class MockWebSocketFactory implements WebSocket {
        public static instance: MockWebSocketFactory | undefined
        public static reset(): void {
            MockWebSocketFactory.readyState = WebSocket.CONNECTING
            MockWebSocketFactory.instance = undefined
        }

        private static readyState: number = WebSocket.CONNECTING
        public static useReadyState(state: number): void {
            MockWebSocketFactory.readyState = state
        }

        CLOSED: number = WebSocket.OPEN
        CLOSING: number = WebSocket.CLOSING
        CONNECTING: number = WebSocket.CONNECTING
        OPEN: number = WebSocket.OPEN

        public url: string
        public readyState: number
        public binaryType: BinaryType = "blob"
        public bufferedAmount = 0
        public extensions = ""
        public protocol: string
        public protocols: string | string[] | undefined

        constructor(url: string, protocols?: string | string[] | undefined) {
            this.url = url
            this.protocol = ""
            this.protocols = protocols
            this.readyState = MockWebSocketFactory.readyState

            MockWebSocketFactory.instance = this
        }

        onclose: ((this: WebSocket, ev: CloseEvent) => void) | null = null
        onerror: ((this: WebSocket, ev: Event) => void) | null = null
        onmessage: ((this: WebSocket, ev: MessageEvent) => void) | null = null
        onopen: ((this: WebSocket, ev: Event) => void) | null = null
        close = jest.fn()
        send = jest.fn()
        addEventListener = jest.fn()
        removeEventListener = jest.fn()
        dispatchEvent = jest.fn()
    }

    afterEach(() => MockWebSocketFactory.reset())

    describe("send", () => {
        it("sends a message over the websocket", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)

            // When
            await connection.start()
            connection.send("hello")

            // Then
            expect(MockWebSocketFactory.instance?.send).toHaveBeenCalledWith(
                "hello",
            )
        })

        it("does not throw an error when the socket is not connected", () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
            )

            // When, Then
            expect(() => connection.send("hello")).not.toThrow()
        })
    })

    describe("asyncIterator", () => {
        it("correctly implements the asyncIterator interface", () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
            )

            // When
            const iterator = connection[Symbol.asyncIterator]()

            // Then
            expect(iterator.next).toBeDefined()
        })

        it("asynchronously returns any messages", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)
            await connection.start()

            // When
            const socket = MockWebSocketFactory.instance

            socket?.onmessage?.(
                new MessageEvent("message", { data: "message" }),
            )
            socket?.onmessage?.(
                new MessageEvent("message", { data: "message_two" }),
            )
            socket?.onmessage?.(
                new MessageEvent("message", { data: "message_three" }),
            )

            const one = await connection.next()
            const two = await connection.next()
            const three = await connection.next()

            // Then
            expect(one).toEqual({
                value: "message",
            })
            expect(two).toEqual({
                value: "message_two",
            })
            expect(three).toEqual({
                value: "message_three",
            })
        })
    })

    describe("start", () => {
        it("starts a websocket connection with the correct settings", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)

            // When
            await connection.start()

            // Then
            const socket = MockWebSocketFactory.instance
            expect(socket?.url).toBe(fakeURL)
            expect(socket?.binaryType).toEqual("blob")
        })

        it("starts a websocket connection with a different binaryType", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
                "arraybuffer",
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)

            // When
            await connection.start()

            // Then
            const socket = MockWebSocketFactory.instance
            expect(socket?.url).toBe(fakeURL)
            expect(socket?.binaryType).toEqual("arraybuffer")
        })

        it("starts listening for messages", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
                "arraybuffer",
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)

            // When
            await connection.start()

            // Then
            const socket = MockWebSocketFactory.instance
            expect(socket?.onmessage).toEqual(expect.any(Function))
        })

        it("starts listening for error messages", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
                "arraybuffer",
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)

            // When
            await connection.start()

            // Then
            const socket = MockWebSocketFactory.instance
            expect(socket?.onerror).toEqual(expect.any(Function))
        })

        it("closes the connection when an error happens", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
                "arraybuffer",
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)

            // When
            await connection.start()
            const socket = MockWebSocketFactory.instance
            socket?.onerror?.(new Event("error"))

            // Then
            expect(socket?.close).toHaveBeenCalled()
            expect(socket?.onmessage).toBeNull()
            expect(socket?.onerror).toBeNull()
            expect(socket?.onclose).toBeNull()
            expect(connection.state).toEqual(ConnectionState.CLOSED)
        })

        it("starts listening for close messages", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
                "arraybuffer",
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)

            // When
            await connection.start()

            // Then
            const socket = MockWebSocketFactory.instance
            expect(socket?.onclose).toEqual(expect.any(Function))
        })

        it("disposes the connection when the socket closes", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
                "arraybuffer",
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)

            // When
            await connection.start()
            const socket = MockWebSocketFactory.instance
            socket?.onclose?.(new CloseEvent("closing"))

            // Then
            expect(socket?.onmessage).toBeNull()
            expect(socket?.onerror).toBeNull()
            expect(socket?.onclose).toBeNull()
            expect(connection.state).toEqual(ConnectionState.CLOSED)
        })

        it("resolves the returned promise when the socket opens", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
                "arraybuffer",
            )

            // When, Then
            const task = connection.start()
            expect(connection.state).toBe(ConnectionState.CONNECTING)

            const socket = MockWebSocketFactory.instance
            socket?.onopen?.(new Event("open"))

            await task
            expect(connection.state).toBe(ConnectionState.OPEN)
        })

        it("rejects the returned promise when the socket closes trying to connect", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
                "arraybuffer",
            )

            // When, Then
            const task = connection.start()
            expect(connection.state).toBe(ConnectionState.CONNECTING)

            const socket = MockWebSocketFactory.instance
            socket?.onclose?.(new CloseEvent("closing"))

            await expect(task).rejects.toBeDefined()
            expect(connection.state).toBe(ConnectionState.CLOSED)
        })
    })

    describe("dispose", () => {
        it("cleans up the connection state", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)

            // When
            await connection.start()
            connection.dispose()

            // Then
            const socket = MockWebSocketFactory.instance
            expect(socket?.onmessage).toBeNull()
            expect(socket?.onerror).toBeNull()
            expect(socket?.onclose).toBeNull()
            expect(connection.state).toEqual(ConnectionState.CLOSED)
        })

        it("disposes the internal message queue", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)

            // When
            await connection.start()
            connection.dispose()

            // Then
            const next = await connection.next()
            expect(next).toEqual({
                done: true,
            })
        })

        it("it possible to dispose multiple times", async () => {
            // Given
            const connection = new WebSocketConnection(
                fakeURL,
                MockWebSocketFactory,
            )
            MockWebSocketFactory.useReadyState(WebSocket.OPEN)

            // When
            await connection.start()

            // Then
            expect(() => {
                connection.dispose()
                connection.dispose()
                connection.dispose()
            }).not.toThrow()
        })
    })
})
