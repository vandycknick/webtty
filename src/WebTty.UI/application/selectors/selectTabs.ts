import { AppState } from "application/actions"

const selectTabs = (state: AppState): AppState["terminal"]["tabs"] =>
    state.terminal.tabs

export default selectTabs
