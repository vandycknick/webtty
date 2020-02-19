import { OpenNewTabReply } from "@webtty/messages"
import { TextDecoder } from "util"

import deserializeMessages from "./deserializeMessages"
import { UnknownMessage } from "./types"

describe("deserializeMessages", () => {
    function hexStringToByte(input: string): Uint8Array {
        if (!input) {
            return new Uint8Array()
        }

        const a = []
        for (let i = 0, len = input.length; i < len; i += 2) {
            a.push(parseInt(input.substr(i, 2), 16))
        }

        return new Uint8Array(a)
    }

    it("deserializes multiple varint encoded messages", () => {
        // Given
        // OpenNewTabReply { id: "7042d4d6-2530-4b4a-a16a-678ace6300c5", parentId: "59014e12-1597-41ea-b29a-8874fe7eb4cf" }
        const openNewTabReplyHex =
            "6a92af4f70656e4e65775461625265706c7982a24964d92437303432643464362d323533302d346234612d613136612d363738616365363330306335a8506172656e744964d92435393031346531322d313539372d343165612d623239612d383837346665376562346366"
        const newTabReply = hexStringToByte(openNewTabReplyHex)

        // StdOutMessage { tabId: "7042d4d6-2530-4b4a-a16a-678ace6300c5", data: "" }
        // StdOutMessage { tabId: "7042d4d6-2530-4b4a-a16a-678ace6300c5", data: "Hello\n" }
        const stdoutHex =
            "4392ab4f75747075744576656e7482a55461624964d92437303432643464362d323533302d346234612d613136612d363738616365363330306335a444617461c4020d0a4892ab4f75747075744576656e7482a55461624964d92437303432643464362d323533302d346234612d613136612d363738616365363330306335a444617461c40768656c6c6f0d0a"
        const stdout = hexStringToByte(stdoutHex)

        const view = new Uint8Array(newTabReply.byteLength + stdout.byteLength)
        view.set(newTabReply, 0)
        view.set(stdout, newTabReply.byteLength)

        // When
        const iterator = deserializeMessages(view.buffer)

        // Then
        const first = iterator.next()
        const second = iterator.next()
        const third = iterator.next()
        const end = iterator.next()

        expect(first.value).toEqual(
            new OpenNewTabReply({
                id: "7042d4d6-2530-4b4a-a16a-678ace6300c5",
                parentId: "59014e12-1597-41ea-b29a-8874fe7eb4cf",
            }),
        )

        expect(second.value.tabId).toBe("7042d4d6-2530-4b4a-a16a-678ace6300c5")
        const decoder = new TextDecoder()
        expect(decoder.decode(new Uint8Array(third.value.data))).toContain(
            "hello",
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
