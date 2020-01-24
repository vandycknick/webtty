import { DeepPartial } from "common/types"
import { cloneDeep, merge } from "lodash/fp"
import { initialState, ThemeState } from "./themeReducer"
import { getCurrentTheme } from "./themeSelectors"

const createState = (state: DeepPartial<ThemeState> = {}): ThemeState => {
    return merge(
        cloneDeep({
            theme: initialState,
        }),
        state,
    )
}

describe("getCurrentTheme", () => {
    it("returns the default theme when no theme is selected", () => {
        // Given
        const state = createState()

        // When
        const selected = getCurrentTheme(state)

        // Then
        expect(selected).toEqual(initialState.themes["default"])
    })

    it("returns the default theme when the selected theme is not installed", () => {
        // Given
        const state = createState({
            theme: { selected: "unkowntheme-2373iufwah72398ew892hjd" },
        })

        // When
        const selected = getCurrentTheme(state)

        // Then
        expect(selected).toEqual(initialState.themes["default"])
    })

    it("returns the selected theme", () => {
        // Given
        const state = createState({
            theme: { selected: "solarized" },
        })

        // When
        const selected = getCurrentTheme(state)

        // Then
        expect(selected).toEqual(initialState.themes["solarized"])
    })
})
