import { ThemeState } from "./themeReducer"
import { ITheme, defaultTheme } from "./theme"

const getSelectedTheme = (state: ThemeState): ITheme => {
    const { installed, selected } = state.theme
    const theme = installed.find(
        (theme) =>
            theme.name.toLocaleLowerCase() === selected.toLocaleLowerCase(),
    )
    return theme ?? defaultTheme
}

export { getSelectedTheme }
