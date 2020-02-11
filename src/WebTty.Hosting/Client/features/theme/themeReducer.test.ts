import themeReducer, { initialState } from "./themeReducer"
import { selectTheme } from "./themeActions"

describe("themeReducer", () => {
    it("returns an initial state", () => {
        // Given, When
        const state = themeReducer(undefined, {} as any)

        // Then
        expect(state).toEqual(initialState)
    })

    it("handles a THEME_SELECTED action", () => {
        // Given
        const action = selectTheme("dark")

        // When
        const state = themeReducer(initialState, action)

        // Then
        expect(state).toEqual({
            ...initialState,
            selected: "dark",
        })
    })
})
