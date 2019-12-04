import AsyncQueue from "../utils/AsyncQueue"
import { IStream } from "./types"
import { IDisposable } from "../types"

class MemoryStream implements IStream<string>, IDisposable {
    public static readonly null = new AsyncQueue<string>()

    private queue: AsyncQueue<string> = new AsyncQueue<string>()

    write(data: string): void {
        this.queue.push(data)
    }

    dispose(): void {
        this.queue.dispose()
    }

    public async *[Symbol.asyncIterator](): AsyncIterableIterator<string> {
        for await (const output of this.queue) {
            yield output
        }
    }
}

export default MemoryStream
