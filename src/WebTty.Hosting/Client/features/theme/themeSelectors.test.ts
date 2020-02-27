import { DeepPartial } from "common/types"
import { cloneDeep, merge } from "lodash/fp"
import { initialState, ThemeState } from "./themeReducer"
import { getSelectedTheme } from "./themeSelectors"
import { defaultTheme } from "./theme"

const createState = (state: DeepPartial<ThemeState> = {}): ThemeState => {
    return merge(
        cloneDeep({
            theme: initialState,
        }),
        state,
    )
}

describe("getSelectedTheme", () => {
    it("returns the default theme when no theme is selected", () => {
        // Given
        const state = createState()

        // When
        const selected = getSelectedTheme(state)

        // Then
        expect(selected).toEqual(defaultTheme)
    })

    it("returns the default theme when the selected theme is not installed", () => {
        // Given
        const state = createState({
            theme: { selected: "unkowntheme-2373iufwah72398ew892hjd" },
        })

        // When
        const selected = getSelectedTheme(state)

        // Then
        expect(selected).toEqual(defaultTheme)
    })

    it("returns the selected theme", () => {
        // Given
        const state = createState({
            theme: {
                selected: "solarized",
                installed: [
                    defaultTheme,
                    { name: "solarized" },
                    { name: "two" },
                    { name: "three" },
                ],
            },
        })

        // When
        const selected = getSelectedTheme(state)

        // Then
        expect(selected).toEqual({ name: "solarized" })
    })

    it("searches a selected theme case insensitive", () => {
        // Given
        const state = createState({
            theme: {
                selected: "mixed case theme",
                installed: [defaultTheme, { name: "MiXed CAse tHeMe" }],
            },
        })

        // When
        const selected = getSelectedTheme(state)

        // Then
        expect(selected).toEqual({ name: "MiXed CAse tHeMe" })
    })
})
