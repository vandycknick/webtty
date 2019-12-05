import {
    TERMINAL_NEW_TAB_CREATED,
    TerminalNewTabCreatedAction,
} from "../actions"

const terminalNewTabCreated = (id: string): TerminalNewTabCreatedAction => ({
    type: TERMINAL_NEW_TAB_CREATED,
    payload: {
        id,
    },
})

export default terminalNewTabCreated
