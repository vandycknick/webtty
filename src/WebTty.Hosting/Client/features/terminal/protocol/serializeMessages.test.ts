import serializeMessage from "./serializeMessage"
import { Messages } from "./types"
import {
    OpenNewTabRequest,
    OpenNewTabReply,
    ResizeTabRequest,
    SendInputRequest,
    OpenOutputRequest,
    OutputEvent,
} from "@webtty/messages"

describe("serializeMessage", () => {
    it.each`
        message
        ${new OpenNewTabRequest()}
        ${new OpenNewTabReply({ id: "123", parentId: "456" })}
        ${new ResizeTabRequest({ tabId: "1", cols: 1, rows: 1 })}
        ${new SendInputRequest({ tabId: "1", payload: "h" })}
        ${new OpenOutputRequest({ tabId: "123" })}
        ${new OutputEvent({ tabId: "123", data: [1, 2, 3] })}
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
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        const message: any = {}

        // When, Then
        expect(() => serializeMessage(message)).toThrow("Unknown message!")
    })
})
