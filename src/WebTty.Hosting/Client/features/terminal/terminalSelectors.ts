import { TerminalState } from "./terminalReducer"

const getSelectedTab = (state: TerminalState): string =>
    state.terminal.selectedTab ?? ""

export { getSelectedTab }
