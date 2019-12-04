import terminalReducer from "./reducers/terminalReducer"
import uiReducer from "./reducers/uiReducer"

export * from "./interfaces"

export const reducers = {
    terminal: terminalReducer,
    ui: uiReducer,
}

export { default as writeInput } from "./services/writeInput"
export { default as openNewTab } from "./services/openNewTab"
export { default as resizeTab } from "./services/resizeTab"

export { default as selectTerminalState } from "./selectors/selectTerminalState"
