import AsyncQueue from "./AsyncQueue"

interface EventEmitter {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    addEventListener(event: string, listener: (...args: any[]) => void): void
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    removeEventListener(event: string, listener: (...args: any[]) => void): void
}

function fromEmitter<T>(
    eventEmitter: EventEmitter,
    data = "message",
    error = "error",
    close = "close",
): AsyncIterable<T> {
    const queue = new AsyncQueue<T>()

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const errorListener = (...args: any[]): void => {
        queue.throw(args[0])
        dispose()
    }
    const doneListener = (): void => {
        queue.dispose()
        dispose()
    }

    const dispose = (): void => {
        eventEmitter.removeEventListener(data, queue.push)
        eventEmitter.removeEventListener(error, errorListener)
        eventEmitter.removeEventListener(close, doneListener)
    }

    eventEmitter.addEventListener(data, queue.push)
    eventEmitter.addEventListener(error, errorListener)
    eventEmitter.addEventListener(close, doneListener)

    return queue
}

export default fromEmitter
