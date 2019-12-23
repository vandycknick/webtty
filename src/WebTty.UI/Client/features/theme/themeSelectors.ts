import theme from "./themeReducer"
import { ITheme } from "./themes"

type ThemeState = { theme: ReturnType<typeof theme> }

const getCurrentTheme = (state: ThemeState): ITheme =>
    state.theme.themes[state.theme.selected] || state.theme.themes["default"]

export { getCurrentTheme }
