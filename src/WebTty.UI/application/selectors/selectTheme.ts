import { AppState } from "application/actions"
import themes, { ITheme } from "application/themes"

const selectTheme = (state: AppState): ITheme => themes[state.ui.theme]

export default selectTheme
