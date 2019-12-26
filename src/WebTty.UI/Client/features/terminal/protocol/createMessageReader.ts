import { Messages } from "./types"
import deserializeMessages from "./deserializeMessages"

type MessageReader = () => AsyncIterableIterator<Messages>

const createMessageReader = (
    dataStream: AsyncIterableIterator<string | ArrayBuffer>,
): MessageReader => {
    async function* reader(): AsyncIterableIterator<Messages> {
        for await (const buffer of dataStream) {
            if (typeof buffer === "string") continue

            for (const message of deserializeMessages(buffer)) {
                yield message
            }
        }
    }

    return reader
}

export default createMessageReader
