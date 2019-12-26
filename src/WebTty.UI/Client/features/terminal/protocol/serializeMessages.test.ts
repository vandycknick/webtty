import serializeMessage from "./serializeMessage"
import { Messages } from "./types"
import {
    OpenNewTabRequest,
    ResizeTabMessage,
    StdInputRequest,
    OpenNewTabReply,
    StdOutMessage,
} from "@webtty/messages"

describe("serializeMessage", () => {
    it.each`
        message
        ${new OpenNewTabRequest()}
        ${new OpenNewTabReply({ id: "123" })}
        ${new ResizeTabMessage({ tabId: "1", cols: 1, rows: 1 })}
        ${new StdInputRequest({ tabId: "1", payload: "h" })}
        ${new StdOutMessage({ tabId: "123", data: [1, 2, 3] })}
    `(
        "serializes a message into an array buffer",
        ({ message }: { message: Messages; expected: ArrayBuffer }): void => {
            // Given, When
            const buffer = serializeMessage(message)

            // Then
            expect(new Uint8Array(buffer)).toMatchSnapshot()
        },
    )

    it("throws an error trying to serialize an unknown message", () => {
        // Given
        const message: any = {}

        // When, Then
        expect(() => serializeMessage(message)).toThrow("Unknown message!")
    })
})
