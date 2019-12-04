import { TerminalSessionAction, TERMINAL_SESSION } from "application/actions"

const terminalSession = (
    state: TerminalSessionAction["state"],
): TerminalSessionAction => ({
    type: TERMINAL_SESSION,
    state,
})

export default terminalSession
