import { Reducer } from "redux"
import { produce } from "immer"
import {
    TerminalState,
    TerminalActions,
    TERMINAL_SESSION,
    TERMINAL_NEW_TAB_CREATED,
} from "application/actions"

const initialState: TerminalState = {
    state: "connecting",
    tabs: [],
}

const terminalReducer: Reducer<TerminalState, TerminalActions> = (
    state = initialState,
    action,
) =>
    produce(state, draft => {
        switch (action.type) {
            case TERMINAL_SESSION:
                draft.state = action.state
                break

            case TERMINAL_NEW_TAB_CREATED:
                draft.tabs.push(action.payload.id)
                break
        }
    })

export default terminalReducer
