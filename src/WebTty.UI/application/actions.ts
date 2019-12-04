const TERMINAL_SESSION = "@@webtty/TERMINAL_SESSION"
const TERMINAL_NEW_TAB = "@@webtty/TERMINAL_NEW_TAB"
const TERMINAL_NEW_TAB_CREATED = "@@webtty/TERMINAL_NEW_TAB_CREATED"

const UI_SET_THEME = "@@webtty/UI_SET_THEME"

type TerminalSessionAction = {
    type: typeof TERMINAL_SESSION
    state: "connecting" | "connected" | "disconnected"
}

type TerminalNewTabAction = {
    type: typeof TERMINAL_NEW_TAB
}

type TerminalNewTabCreatedAction = {
    type: typeof TERMINAL_NEW_TAB_CREATED
    payload: { id: string }
}

type TerminalActions =
    | TerminalSessionAction
    | TerminalNewTabAction
    | TerminalNewTabCreatedAction

type UISetThemeAction = {
    type: typeof UI_SET_THEME
    theme: "default" | "solarized"
}

type UIActions = UISetThemeAction

type TerminalState = {
    state: TerminalSessionAction["state"]
    tabs: string[]
}

type UIState = {
    theme: UISetThemeAction["theme"]
}

type AppState = {
    terminal: TerminalState
    ui: UIState
}

export {
    TERMINAL_SESSION,
    TERMINAL_NEW_TAB,
    TERMINAL_NEW_TAB_CREATED,
    UI_SET_THEME,
}
export {
    TerminalSessionAction,
    TerminalNewTabAction,
    TerminalNewTabCreatedAction,
    TerminalActions,
    UISetThemeAction,
    UIActions,
}
export { AppState, TerminalState, UIState }
