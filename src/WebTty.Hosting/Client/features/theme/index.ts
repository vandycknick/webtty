import { ITheme, defaultTheme } from "./theme"
import { ThemeState } from "./themeReducer"

const createThemeState = (
    selected = "default",
    themes: ITheme[] = [],
): ThemeState => ({
    theme: {
        selected,
        installed: [defaultTheme, ...themes],
    },
})

export { createThemeState, ITheme }
