import createMessageReader from "./createMessageReader"
import AsyncQueue from "common/async/AsyncQueue"
import { OpenNewTabReply, StdOutMessage } from "@webtty/messages"
import { UnknownMessage } from "./types"

describe("createMessageReader", () => {
    const newTabReply = new Uint8Array([
        25,
        146,
        175,
        79,
        112,
        101,
        110,
        78,
        101,
        119,
        84,
        97,
        98,
        82,
        101,
        112,
        108,
        121,
        129,
        162,
        73,
        100,
        163,
        49,
        50,
        51,
    ])
    const stdout = new Uint8Array([
        37,
        146,
        173,
        83,
        116,
        100,
        79,
        117,
        116,
        77,
        101,
        115,
        115,
        97,
        103,
        101,
        130,
        165,
        84,
        97,
        98,
        73,
        100,
        163,
        49,
        50,
        51,
        164,
        68,
        97,
        116,
        97,
        149,
        72,
        101,
        108,
        108,
        111,
    ])
    const unknown = new Uint8Array([8, 146, 165, 104, 101, 108, 108, 111, 128])

    it("creates a reader that parses messages from a stream of array buffer", async () => {
        // Given
        const queue = new AsyncQueue<ArrayBuffer>()
        const reader = createMessageReader(queue)
        const view = new Uint8Array(newTabReply.byteLength + stdout.byteLength)
        view.set(newTabReply, 0)
        view.set(stdout, newTabReply.byteLength)

        // When
        queue.enqueue(view)
        queue.enqueue(unknown)
        queue.enqueue(stdout)
        queue.enqueue(newTabReply)
        queue.dispose()

        // Then
        const results = [
            OpenNewTabReply,
            StdOutMessage,
            UnknownMessage,
            StdOutMessage,
            OpenNewTabReply,
        ]
        let cnt = 0

        for await (const message of reader()) {
            expect(message).toBeInstanceOf(results[cnt])
            cnt++
        }

        expect(cnt).toEqual(5)
    })

    it("skips parsing messages that have a string type", async () => {
        // Given
        const queue = new AsyncQueue<string | ArrayBuffer>()
        const reader = createMessageReader(queue)

        // When
        queue.enqueue("hello")
        queue.enqueue(unknown)
        queue.dispose()

        // Then
        let cnt = 0
        for await (const message of reader()) {
            expect(message).toBeInstanceOf(UnknownMessage)
            cnt++
        }
        expect(cnt).toEqual(1)
    })
})
