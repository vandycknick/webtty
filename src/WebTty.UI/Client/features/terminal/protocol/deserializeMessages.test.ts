import { OpenNewTabReply } from "@webtty/messages"
import { TextDecoder } from "util"

import deserializeMessages from "./deserializeMessages"
import { UnknownMessage } from "./types"

describe("deserializeMessages", () => {
    it("deserializes multiple varint encoded messages", () => {
        // Given
        // OpenNewTabReply { id: "123" }
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

        // StdOutMessage { tabId: "123", data: "Hello" }
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
        const view = new Uint8Array(newTabReply.byteLength + stdout.byteLength)
        view.set(newTabReply, 0)
        view.set(stdout, newTabReply.byteLength)

        // When
        const iterator = deserializeMessages(view.buffer)

        // Then
        const first = iterator.next()
        const second = iterator.next()
        const end = iterator.next()

        expect(first.value).toEqual(new OpenNewTabReply({ id: "123" }))

        expect(second.value.tabId).toBe("123")
        const decoder = new TextDecoder()
        expect(decoder.decode(new Uint8Array(second.value.data))).toEqual(
            "Hello",
        )

        expect(end).toEqual({
            done: true,
            value: undefined,
        })
    })

    it("returns an unknown message when trying to deserialize an unknown payload", () => {
        // Given
        // ["hello", {}]
        const payload = new Uint8Array([
            8,
            146,
            165,
            104,
            101,
            108,
            108,
            111,
            128,
        ])

        // When
        const result = deserializeMessages(payload).next()

        // Then
        expect(result.value).toEqual(new UnknownMessage("hello", {}))
    })
})
