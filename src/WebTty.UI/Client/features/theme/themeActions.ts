import { Action } from "redux"

const THEME_SELECTED = "@webtty/THEME_SELECTED"

type SelectedThemeAction = Action<typeof THEME_SELECTED> & {
    payload: string
}

type ThemeActions = SelectedThemeAction

const selectTheme = (theme: string): SelectedThemeAction => ({
    type: THEME_SELECTED,
    payload: theme,
})

export { THEME_SELECTED }
export { ThemeActions }
export { selectTheme }
