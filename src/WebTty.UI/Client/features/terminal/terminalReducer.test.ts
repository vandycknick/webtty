import terminalReducer, { initialState } from "./terminalReducer"
import { setStatus, newTab } from "./terminalActions"

describe("terminalReducer", () => {
    it("should return an initial state", () => {
        // Given, When
        const state = terminalReducer(undefined, {} as any)

        // Then
        expect(state).toEqual(initialState)
    })

    it("handles a TERMINAL_SET_STATE action", () => {
        // Given
        const action = setStatus("connecting")

        // When
        const state = terminalReducer(initialState, action)

        // Then
        expect(state).toEqual({
            status: "connecting",
            tabs: [],
        })
    })

    it("handles a TERMINAL_TAB_CREATED action", () => {
        // Given
        const action = newTab("123")

        // When
        const state = terminalReducer(initialState, action)

        // Then
        expect(state).toEqual({
            ...initialState,
            selectedTab: "123",
            tabs: ["123"],
        })
    })

    it("handles multiple TERMINAL_TAB_CREATED actions", () => {
        // Given
        const action = newTab("456")
        const currentState = {
            ...initialState,
            selectedTab: "123",
            tabs: ["123"],
        }

        // When
        const state = terminalReducer(currentState, action)

        // Then
        expect(state).toEqual({
            ...initialState,
            selectedTab: "456",
            tabs: ["123", "456"],
        })
    })
})
