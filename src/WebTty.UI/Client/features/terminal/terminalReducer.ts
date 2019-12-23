import { Reducer } from "redux"
import { produce } from "immer"
import {
    TerminalActions,
    TERMINAL_SET_STATUS,
    TERMINAL_NEW_TAB,
} from "./terminalActions"

type TerminalState = {
    status: "idle" | "connecting" | "connected"
    tabs: string[]
    selectedTab?: string
}

const initialState: TerminalState = {
    status: "idle",
    tabs: [],
}

const terminal: Reducer<TerminalState, TerminalActions> = (
    state = initialState,
    action,
) =>
    produce(state, draft => {
        switch (action.type) {
            case TERMINAL_SET_STATUS:
                draft.status = action.payload
                break

            case TERMINAL_NEW_TAB:
                draft.tabs.push(action.payload.id)
                draft.selectedTab = action.payload.id
                break
        }
    })

export default terminal
