import Deferred from "./Deferred"

const $terminated = Symbol.for("terminated")

class AsyncQueue<T> {
    private buffer: (T | typeof $terminated)[] = []
    private isDisposed = false
    private errorMessage?: Error
    private delayMessage?: Deferred<T | typeof $terminated> = undefined

    public push = (data: T): void => {
        if (this.delayMessage === undefined) {
            this.buffer.push(data)
            return
        }

        this.delayMessage.resolve(data)
        this.delayMessage = undefined
    }

    public throw = (error: Error): void => {
        if (this.delayMessage === undefined) {
            this.errorMessage = error
            return
        }

        this.delayMessage.reject(error)
        this.delayMessage = undefined
    }

    public dispose = (): void => {
        if (this.delayMessage !== undefined) {
            this.delayMessage.resolve($terminated)
        }

        if (this.isDisposed) return

        this.isDisposed = true
    }

    public async *[Symbol.asyncIterator](): AsyncIterableIterator<T> {
        while (!this.isDisposed) {
            if (this.errorMessage) {
                this.dispose()
                throw this.errorMessage
            }

            if (this.buffer.length === 0) {
                if (this.isDisposed) {
                    this.dispose()
                    return
                }
                this.delayMessage = new Deferred<T | typeof $terminated>()
                const data = await this.delayMessage

                if (data === $terminated) {
                    this.dispose()
                    return
                }

                yield data
            } else {
                const data = this.buffer.shift()

                if (data === $terminated) {
                    this.dispose()
                    return
                }

                if (data !== undefined) {
                    yield data
                }
            }
        }
    }
}

export default AsyncQueue
