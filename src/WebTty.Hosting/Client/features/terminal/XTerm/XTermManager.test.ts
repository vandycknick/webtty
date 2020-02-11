import XTerm from "features/terminal/XTerm/XTerm"
import XTermManager from "./XTermManager"

jest.mock("features/terminal/XTerm/XTerm")

describe("XTermManager", () => {
    afterEach(() => jest.resetAllMocks())

    it("creates a new terminal when not present in storage", () => {
        // Given
        const termManager = new XTermManager()

        // When
        const term = termManager.get("123")

        // Then
        expect(XTerm).toHaveBeenCalledTimes(1)
        expect(term).toBeDefined()
    })

    it("returns a cached terminal when present in storage", () => {
        // Given
        const termManager = new XTermManager()

        // When
        const term = termManager.get("123")
        const term2 = termManager.get("123")

        // Then
        expect(XTerm).toHaveBeenCalledTimes(1)
        expect(term).toEqual(term2)
    })

    it("writes data to the terminal", () => {
        // Given
        const termManager = new XTermManager()

        // When
        const term = termManager.get("123")
        termManager.write("123", "hello")

        // Then
        expect(term.write).toHaveBeenCalledWith("hello")
    })

    it("ignores writing when no terminal with the given id is found", () => {
        // Given
        const termManager = new XTermManager()

        // When, Then
        expect(() => {
            termManager.write("123", "hello")
        }).not.toThrow()
    })
})
