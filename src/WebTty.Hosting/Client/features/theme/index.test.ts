import { createThemeState } from "."
import { defaultTheme, ITheme } from "./theme"

describe("createThemeState", () => {
    it("returns a default state", () => {
        // Given, When
        const state = createThemeState()

        // Then
        expect(state).toEqual({
            theme: {
                selected: "default",
                installed: [defaultTheme],
            },
        })
    })

    it("returns a state with a selected theme", () => {
        // Given
        const selected = "solarized"

        // When
        const state = createThemeState(selected)

        // Then
        expect(state).toEqual({
            theme: {
                selected,
                installed: [defaultTheme],
            },
        })
    })

    it("returns a state with correct installed themes", () => {
        // Given
        const themes: ITheme[] = [
            { name: "one", background: "#fff" },
            { name: "two", background: "#fff" },
            { name: "three", background: "#fff" },
        ]

        // When
        const state = createThemeState(undefined, themes)

        // Then
        expect(state).toEqual({
            theme: {
                selected: "default",
                installed: [defaultTheme, ...themes],
            },
        })
    })
})
