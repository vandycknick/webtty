import { UISetThemeAction, UI_SET_THEME } from "application/actions"

const setTheme = (theme = "default"): UISetThemeAction => ({
    type: UI_SET_THEME,
    theme,
})

export default setTheme
