import AsyncQueue from "./AsyncQueue"

describe("AsyncQueue", () => {
    const delay = (time?: number): Promise<void> =>
        new Promise(resolve => {
            setTimeout(resolve, time)
        })

    it("returns an enqueued value", async () => {
        // Given
        const queue = new AsyncQueue<string>()

        // When
        queue.enqueue("hello")

        // Then
        const message = await queue.next()
        expect(message.done).toBeFalsy()
        expect(message.value).toBe("hello")
    })

    it("throws an error message when an enqueued value is an error", async () => {
        // Given
        const queue = new AsyncQueue<string>()

        // When
        queue.enqueue(new Error("error"))

        // Then
        await expect(queue.next()).rejects.toThrow("error")
    })

    it("returns a done iterator return result and undefined when disposed", async () => {
        // Given
        const queue = new AsyncQueue<string>()

        // When
        queue.dispose()

        // Then
        const message = await queue.next()
        expect(message).toEqual({
            done: true,
            value: undefined,
        })
    })

    it("returns a done iterator result calling next multiple times on a disposed queue", async () => {
        // Given
        const queue = new AsyncQueue<string>()

        // When
        queue.dispose()

        // Then
        const message = await queue.next()
        const messageTwo = await queue.next()
        expect(message).toEqual({
            done: true,
            value: undefined,
        })
        expect(messageTwo).toEqual({
            done: true,
            value: undefined,
        })
    })

    it("resolves every pending deferred when multiple are registered and the queue is being disposed", async () => {
        // Given
        const queue = new AsyncQueue<string>()

        // When
        const first = queue.next()
        const second = queue.next()
        const third = queue.next()
        queue.dispose()

        // Then
        await expect(first).resolves.toEqual({
            done: true,
            value: undefined,
        })
        await expect(second).resolves.toEqual({
            done: true,
            value: undefined,
        })
        await expect(third).resolves.toEqual({
            done: true,
            value: undefined,
        })
    })

    it("throws an error when trying to enqueue a message to a disposed queue", () => {
        // Given
        const queue = new AsyncQueue<string>()

        // When
        queue.dispose()

        // Then
        expect(() => queue.enqueue("error")).toThrow("Closed")
    })

    it("returns an unresolved promise when calling next and there is no value in the queue", async () => {
        // Given
        const queue = new AsyncQueue<string>()
        let isResolved = false

        // When, Then
        const promise = queue.next().finally(() => (isResolved = true))

        await delay()

        expect(isResolved).toBe(false)

        queue.enqueue("hello")
        queue.enqueue("world")

        await expect(promise).resolves.toEqual({ value: "hello" })
        expect(isResolved).toBe(true)
    })

    it("rejects an unresolved promise when calling next and a future enqueued value is an error", async () => {
        // Given
        const queue = new AsyncQueue<string>()
        let isResolved = false

        // When, Then
        const promise = queue.next().finally(() => (isResolved = true))

        await delay()

        expect(isResolved).toBe(false)

        queue.enqueue(new Error("hello"))

        await expect(promise).rejects.toThrow("hello")
        expect(isResolved).toBe(true)
    })

    it("returns messages in the same order as they were enqueued", async () => {
        // Given
        const queue = new AsyncQueue<string>()
        const messages = ["first", "second", "third", "fourth", "fifth"]

        // When, Then
        const iterator = (async function() {
            let matcher = 0
            for await (const message of queue) {
                expect(message).toBe(messages[matcher])
                matcher++
            }
        })()

        for (const message of messages) {
            queue.enqueue(message)
        }

        queue.dispose()
        await iterator
    })
})
