const $terminated = Symbol.for("terminated")
const $listeners = Symbol.for("$parent")

type Resolver<T> = (value?: T | PromiseLike<T> | undefined) => void
type Rejector<T> = (reason?: T) => void

class Deferred<T> implements Promise<T> {
    private _resolveSelf: Resolver<T> = Promise.resolve
    private _rejectSelf: Rejector<T> = Promise.reject
    private promise: Promise<T>

    public constructor() {
        this.promise = new Promise<T>((resolve, reject): void => {
            this._resolveSelf = resolve
            this._rejectSelf = reject
        })
    }

    public then<TResult1 = T, TResult2 = never>(
        onfulfilled?: ((value: T) => TResult1 | PromiseLike<TResult1>) | undefined | null,
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        onrejected?: ((reason: any) => TResult2 | PromiseLike<TResult2>) | undefined | null,
    ): Promise<TResult1 | TResult2> {
        return this.promise.then(onfulfilled, onrejected)
    }

    public catch<TResult = never>(
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        onrejected?: ((reason: any) => TResult | PromiseLike<TResult>) | undefined | null,
    ): Promise<T | TResult> {
        return this.promise.then(onrejected)
    }

    public finally(onfinally?: (() => void) | null | undefined): Promise<T> {
        return this.promise.finally(onfinally)
    }

    public resolve(val: T): void {
        this._resolveSelf(val)
    }

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    public reject(reason: any): void {
        this._rejectSelf(reason)
    }

    public [Symbol.toStringTag]: "Promise"
}

class AsyncQueue<T> {
    public static from<T>(iterator: AsyncIterable<T>): AsyncQueue<T> {
        if ($listeners in iterator) {
            const listeners = (iterator as any)[$listeners] as AsyncQueue<T>[]
            const queue = new AsyncQueue<T>()
            listeners.push(queue)
            return queue
        } else {
            const listeners = ((iterator as any)[$listeners] = [] as AsyncQueue<T>[])
            const queue = new AsyncQueue<T>()
            listeners.push(queue)
            ;(async function(): Promise<void> {
                try {
                    for await (let message of iterator) {
                        listeners.forEach(l => {
                            if (l.isDisposed) return

                            l.push(message)
                        })
                    }
                } catch (ex) {
                    listeners.forEach(l => l.throw(ex))
                } finally {
                    listeners.forEach(l => l.dispose())
                }
            })()
            return queue
        }
    }

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
