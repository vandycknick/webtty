import ConnectionBuilder from "./ConnectionBuilder"
import WebSocketConnection from "./WebSocketConnection"

jest.mock("./WebSocketConnection")

describe("ConnectionBuilder", () => {
    afterEach(() => {
        jest.resetAllMocks()
        jest.clearAllMocks()
    })

    it("should create a websocket connection with the given url", () => {
        // Given
        const url = "/dummy-endpoint"

        // When
        new ConnectionBuilder()
            .withUrl(url)
            .useWebSocket()
            .build()

        // Then
        expect(WebSocketConnection).toHaveBeenCalledWith(
            url,
            expect.any(Function),
            undefined,
        )
    })

    it("should create a websocket connection with the given binarytype", () => {
        // Given
        const url = "/dummy-endpoint"

        // When
        new ConnectionBuilder()
            .withUrl(url)
            .useWebSocket("arraybuffer")
            .build()

        // Then
        expect(WebSocketConnection).toHaveBeenCalledWith(
            url,
            expect.any(Function),
            "arraybuffer",
        )
    })

    it("throws an exception when method is used that defines the connectionFactory", () => {
        // Given
        const url = "/hello"

        // When, Then
        expect(() => new ConnectionBuilder().withUrl(url).build()).toThrow(
            "No default connection provided",
        )
    })
})
