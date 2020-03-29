import { Reducer } from "redux"
import { produce } from "immer"
import {
    TerminalActions,
    TERMINAL_SET_STATUS,
    TERMINAL_TAB_CREATED,
} from "./terminalActions"

type State = {
    status: "idle" | "connecting" | "connected"
    tabs: string[]
    selectedTab?: string
}

const initialState: State = {
    status: "idle",
    tabs: [],
}

const terminal: Reducer<State, TerminalActions> = (
    state = initialState,
    action,
) =>
    produce(state, (draft) => {
        switch (action.type) {
            case TERMINAL_SET_STATUS:
                draft.status = action.payload
                break

            case TERMINAL_TAB_CREATED:
                draft.tabs.push(action.payload.id)
                draft.selectedTab = action.payload.id
                break
        }
    })

type TerminalState = { terminal: ReturnType<typeof terminal> }

export { TerminalState, initialState }
export default terminal
