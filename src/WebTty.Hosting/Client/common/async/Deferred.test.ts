import Deferred from "./Deferred"

describe("Deferred", () => {
    it("implements a promisable interface", () => {
        // Given
        const deferred = new Deferred()

        // When, Then
        expect(deferred[Symbol.toStringTag]).toEqual("Promise")
        expect(deferred.then).toEqual(expect.any(Function))
        expect(deferred.catch).toEqual(expect.any(Function))
        expect(deferred.finally).toEqual(expect.any(Function))
    })

    it("resolve the deferred promise when resolve is called", async () => {
        // Given
        const deferred = new Deferred<string>()

        // When
        deferred.resolve("done")

        // Then
        await expect(deferred).resolves.toEqual("done")
    })

    it("rejects the deferred promise when reject is called", async () => {
        // Given
        const deferred = new Deferred<string>()

        // When
        deferred.reject("error")

        // Then
        await expect(deferred).rejects.toEqual("error")
    })

    it("calls any registered thenable handlers when the deferred promise resolves", async () => {
        // Given
        const deferred = new Deferred<string>()
        const one = jest.fn()
        deferred.then(one)

        // When
        deferred.resolve("hello")
        await deferred

        // Then
        expect(one).toHaveBeenCalledWith("hello")
    })

    it("calls any registered error handler when the deferred promise rejects", async () => {
        // Given
        const deferred = new Deferred<string>()
        const one = jest.fn()
        deferred.catch(one)

        // When
        deferred.reject("error")
        await expect(deferred).rejects

        // Then
        expect(one).toHaveBeenCalledWith("error")
    })

    it("calls any registered finally handler when the promise resolve or rejects", async () => {
        // Given
        const resolve = new Deferred<string>()
        const rejects = new Deferred<string>()

        const one = jest.fn()
        const two = jest.fn()
        resolve.finally(one)
        rejects.catch(jest.fn()).finally(two)

        // When
        resolve.resolve("one")
        rejects.reject("two")
        await expect(Promise.all([resolve, rejects])).rejects.toEqual("two")

        // Then
        expect(one).toHaveBeenCalled()
        expect(two).toHaveBeenCalled()
    })
})
