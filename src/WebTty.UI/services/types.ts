const TERMINAL_SESSION = "@@webtty/TERMINAL_SESSION"
const TERMINAL_SESSION_DISCONNECTED = "@@webtty/TERMINAL_SESSION_DISCONNECTED"

const TERMINAL_NEW_TAB = "@@webtty/TERMINAL_NEW_TAB"
const TERMINAL_NEW_TAB_CREATED = "@@webtty/TERMINAL_NEW_TAB_CREATED"

type TerminalSessionAction = {
    type: typeof TERMINAL_SESSION
}

type TerminalSessionDisconnectedAction = {
    type: typeof TERMINAL_SESSION_DISCONNECTED
}

type TerminalNewTabAction = {
    type: typeof TERMINAL_NEW_TAB
}

type TerminalNewTabCreatedAction = {
    type: typeof TERMINAL_NEW_TAB_CREATED
    payload: { id: number }
}

type TerminalActions =
    | TerminalSessionAction
    | TerminalSessionDisconnectedAction
    | TerminalNewTabAction
    | TerminalNewTabCreatedAction

type TerminalState = {
    tabId: number | undefined
}

type AppState = TerminalState

export {
    TERMINAL_SESSION,
    TERMINAL_SESSION_DISCONNECTED,
    TERMINAL_NEW_TAB,
    TERMINAL_NEW_TAB_CREATED,
    TerminalActions,
    TerminalState,
    AppState,
}
