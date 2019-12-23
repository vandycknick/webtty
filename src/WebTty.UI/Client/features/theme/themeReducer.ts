import produce from "immer"
import { Reducer } from "redux"

import { ThemeActions, THEME_SELECTED } from "./themeActions"
import themes, { ITheme } from "./themes"

type ThemeState = {
    selected: string
    themes: {
        [theme: string]: ITheme
    }
}

const initialState: ThemeState = {
    selected: "default",
    themes: {
        ...themes,
    },
}

const theme: Reducer<ThemeState, ThemeActions> = (
    state = initialState,
    action,
) =>
    produce(state, draft => {
        switch (action.type) {
            case THEME_SELECTED:
                draft.selected = action.payload
                break
        }
    })

export default theme
