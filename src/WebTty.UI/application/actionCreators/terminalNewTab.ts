import { TERMINAL_NEW_TAB, TerminalNewTabAction } from "../actions"

const terminalNewTab = (): TerminalNewTabAction => ({
    type: TERMINAL_NEW_TAB,
})

export default terminalNewTab
