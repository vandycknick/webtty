import { AppState } from "../actions"

const selectTerminalState = (state: AppState): AppState["terminal"]["state"] =>
    state.terminal.state

export default selectTerminalState
