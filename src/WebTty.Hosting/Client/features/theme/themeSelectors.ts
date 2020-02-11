import { ThemeState } from "./themeReducer"
import { ITheme } from "./themes"

const getCurrentTheme = (state: ThemeState): ITheme =>
    state.theme.themes[state.theme.selected] || state.theme.themes["default"]

export { getCurrentTheme }
