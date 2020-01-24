import { selectTheme, THEME_SELECTED } from "./themeActions"

describe("selectTheme", () => {
    it("creates a THEME_SELECTED action", () => {
        // Given
        const theme = "dark"

        // When
        const action = selectTheme(theme)

        // Then
        expect(action).toEqual({
            type: THEME_SELECTED,
            payload: theme,
        })
    })
})
