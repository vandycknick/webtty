import { CancellationTokenSource, CancellationToken } from "./CancellationToken"

describe("CancellationTokenSource", () => {
    it("returns a CancellationToken", () => {
        // Given
        const source = new CancellationTokenSource()

        // When
        const token = source.token

        // Then
        expect(token).toBeInstanceOf(CancellationToken)
        expect(token.isCancelled).toBe(false)
    })

    it("cancels the CancellationToken upon request", async () => {
        // Given
        const source = new CancellationTokenSource()

        // When
        source.cancel()

        // Then
        await expect(source.token.promise()).resolves.toBeUndefined()
        expect(source.token.isCancelled).toBe(true)
    })

    it("cancels the CancellationToken when disposed", async () => {
        // Given
        const source = new CancellationTokenSource()

        // When
        source.dispose()

        // Then
        await expect(source.token.promise()).resolves.toBeUndefined()
        expect(source.token.isCancelled).toBe(true)
    })

    it("returns false when cancellation is not requested", () => {
        // Given
        const source = new CancellationTokenSource()

        // When, Then
        expect(source.isCancellationRequested).toBe(false)
    })

    it("returns true when cancellation is requested", async () => {
        // Given
        const source = new CancellationTokenSource()

        // When
        source.cancel()

        // Then
        await source.token.promise()
        expect(source.isCancellationRequested).toBe(true)
    })
})
