const TERMINAL_SESSION = "@@webtty/TERMINAL_SESSION"
const TERMINAL_SESSION_DISCONNECTED = "@@webtty/TERMINAL_SESSION_DISCONNECTED"

const TERMINAL_NEW_TAB = "@@webtty/TERMINAL_NEW_TAB"

type TerminalSessionAction = {
    type: typeof TERMINAL_SESSION
}

type TerminalSessionDisconnectedAction = {
    type: typeof TERMINAL_SESSION_DISCONNECTED
}

type TerminalNewTabAction = {
    type: typeof TERMINAL_NEW_TAB
    payload: { id: number }
}

type TerminalActions = TerminalSessionAction | TerminalSessionDisconnectedAction | TerminalNewTabAction

type TerminalState = {}

type AppState = {
    terminal: TerminalState
}

type AppConfig = {
    socketUrl: string
}

export {
    TERMINAL_SESSION,
    TERMINAL_SESSION_DISCONNECTED,
    TERMINAL_NEW_TAB,

    TerminalActions,

    TerminalState,
    AppState,
    AppConfig,
}
