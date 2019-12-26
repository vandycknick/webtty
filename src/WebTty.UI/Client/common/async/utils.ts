import { CancellationToken } from "common/async/CancellationToken"

const consume = async <T>(
    dataStream: AsyncIterableIterator<T>,
    handler: (data: T) => T,
    token: CancellationToken,
): Promise<void> => {
    while (!token.isCancelled) {
        const result = await Promise.race([dataStream.next(), token.promise()])
        if (result === undefined || result.done) {
            return
        }
        handler(result.value)
    }
}

export { consume }
