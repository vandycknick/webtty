import { AnyAction } from "redux"
import { produce } from "immer"

import { TerminalState } from "./types"

const terminal = (state: TerminalState = {}, _: AnyAction) =>
    produce(state, (_) => {

    })

export { terminal }
