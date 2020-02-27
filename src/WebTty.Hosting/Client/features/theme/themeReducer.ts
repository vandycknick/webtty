import produce from "immer"
import { Reducer } from "redux"

import { ThemeActions, THEME_SELECTED } from "./themeActions"
import { ITheme, defaultTheme } from "./theme"

type State = {
    selected: string
    installed: ITheme[]
}

const initialState: State = {
    selected: "default",
    installed: [defaultTheme],
}

const theme: Reducer<State, ThemeActions> = (state = initialState, action) =>
    produce(state, draft => {
        switch (action.type) {
            case THEME_SELECTED:
                draft.selected = action.payload
                break
        }
    })

type ThemeState = { theme: ReturnType<typeof theme> }

export { ThemeState, initialState }
export default theme
