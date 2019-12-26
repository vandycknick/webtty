import Deferred from "./Deferred"
import { IDisposable } from "../types"

class Queue<T> {
    private readonly _queue: T[] = []

    public get length(): number {
        return this._queue.length
    }

    public enqueue(value: T): number {
        return this._queue.push(value)
    }

    public dequeue(): T | undefined {
        return this._queue.shift()
    }
}

export class AsyncQueue<T>
    implements IDisposable, AsyncIterator<T, undefined>, AsyncIterable<T> {
    private readonly _values: Queue<T | Error>
    private readonly _settlers: Queue<Deferred<IteratorResult<T>>>
    private _disposed: boolean

    constructor() {
        // enqueues > dequeues
        this._values = new Queue()
        // dequeues > enqueues
        this._settlers = new Queue()
        this._disposed = false
    }

    public [Symbol.asyncIterator] = (): AsyncIterableIterator<T> => this

    public enqueue = (value: T | Error): void => {
        if (this._disposed) {
            throw new Error("Closed")
        }
        if (this._settlers.length > 0) {
            if (this._values.length > 0) {
                throw new Error("Illegal internal state")
            }
            const settler = this._settlers.dequeue()
            if (value instanceof Error) {
                settler?.reject(value)
            } else {
                settler?.resolve({ value })
            }
        } else {
            this._values.enqueue(value)
        }
    }

    public next = (): Promise<IteratorResult<T, undefined>> => {
        const value = this._values.dequeue()
        if (value !== undefined) {
            if (value instanceof Error) {
                return Promise.reject(value)
            } else {
                return Promise.resolve({ value })
            }
        }
        if (this._disposed) {
            if (this._settlers.length > 0) {
                throw new Error("Illegal internal state")
            }
            return Promise.resolve({ done: true, value: undefined })
        }
        // Wait for new values to be enqueued
        const deferred = new Deferred<IteratorResult<T, undefined>>()
        this._settlers.enqueue(deferred)
        return deferred
    }

    public dispose = (): void => {
        while (this._settlers.length > 0) {
            this._settlers.dequeue()?.resolve({ done: true, value: undefined })
        }
        this._disposed = true
    }
}

export default AsyncQueue
