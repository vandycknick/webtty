import AsyncQueue from "./AsyncQueue"
import { consume } from "./utils"
import { CancellationTokenSource } from "./CancellationToken"

describe("consume", () => {
    it("passes every message from the given iterable stream to the handler", async () => {
        // Given
        const queue = new AsyncQueue<string>()
        const tokenSource = new CancellationTokenSource()
        const handler = jest.fn()

        // When
        queue.enqueue("one")
        queue.enqueue("two")
        queue.enqueue("three")
        queue.dispose()

        await consume(queue, handler, tokenSource.token)

        // Then
        expect(handler).toHaveBeenNthCalledWith(1, "one")
        expect(handler).toHaveBeenNthCalledWith(2, "two")
        expect(handler).toHaveBeenNthCalledWith(3, "three")
    })

    it("stops iteration when the cancellation token is cancelled", async () => {
        // Given
        const queue = new AsyncQueue<string>()
        const tokenSource = new CancellationTokenSource()
        const handler = jest.fn()

        // When
        queue.enqueue("one")
        queue.enqueue("two")
        queue.enqueue("three")

        // When
        let running = true
        const job = (async () => {
            await consume(queue, handler, tokenSource.token)
            running = false
        })()

        // Then
        expect(running).toBe(true)
        tokenSource.cancel()
        await job
        expect(running).toBe(false)
        expect(handler).toHaveBeenCalled()
        queue.dispose()
    })
})
