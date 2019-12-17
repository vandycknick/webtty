import { Reducer } from "redux"
import { UIState, UIActions, UI_SET_THEME } from "application/actions"
import produce from "immer"

const initialState: UIState = {
    theme: "default",
}

const uiReducer: Reducer<UIState, UIActions> = (state = initialState, action) =>
    produce(state, draft => {
        switch (action.type) {
            case UI_SET_THEME:
                draft.theme = action.theme
        }
    })

export default uiReducer
