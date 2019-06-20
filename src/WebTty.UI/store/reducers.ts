import { produce } from "immer"

import { TerminalState, TerminalActions, TERMINAL_NEW_TAB_CREATED } from "./types"

const terminal = (state: TerminalState = { tabId: undefined}, action: TerminalActions) =>
    produce(state, (draft) => {
        switch (action.type) {
            case TERMINAL_NEW_TAB_CREATED:
                draft.tabId = action.payload.id;
        }
    })

export { terminal }
