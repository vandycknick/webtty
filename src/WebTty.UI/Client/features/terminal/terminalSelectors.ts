import terminal from "./terminalReducer"

type State = { terminal: ReturnType<typeof terminal> }

const getSelectedTab = (state: State): string =>
    state.terminal.selectedTab ?? ""

export { getSelectedTab }
