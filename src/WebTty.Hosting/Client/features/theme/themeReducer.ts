import produce from "immer"
import { Reducer } from "redux"

import { ThemeActions, THEME_SELECTED } from "./themeActions"
import themes, { ITheme } from "./themes"

type State = {
    selected: string
    themes: {
        [theme: string]: ITheme
    }
}

const initialState: State = {
    selected: "default",
    themes: {
        ...themes,
    },
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
