import createMessageWriter from "./createMessageWriter"
import { IConnection } from "common/connection"
import { ConnectionState } from "common/connection/IConnection"
import { OpenNewTabReply } from "@webtty/messages"

describe("createMessageWriter", () => {
    it("creates a message writer that send any message serialized over the given connection", () => {
        // Given
        const connection: IConnection = {
            state: ConnectionState.OPEN,
            start: jest.fn(),
            send: jest.fn(),
            next: jest.fn(),
            dispose: jest.fn(),
            [Symbol.asyncIterator]: jest.fn(),
        }
        const writer = createMessageWriter(connection)

        // When
        const msg = new OpenNewTabReply({ id: "123", parentId: "456" })
        writer(msg)

        // Then
        expect(connection.send).toHaveBeenCalledWith(expect.any(ArrayBuffer))
    })
})
