import { ThunkAction } from "redux-thunk"

import { OpenNewTabReply, StdOutMessage } from "@webtty/messages"
import terminalNewTabCreated from "application/actionCreators/terminalNewTabCreated"
import { IDisposable } from "lib/types"
import { CancellationTokenSource } from "lib/utils/CancellationToken"

import { TerminalActions } from "../actions"
import { AppServices } from "../interfaces"

const processTty = (): ThunkAction<
    IDisposable,
    null,
    AppServices,
    TerminalActions
> => (dispatch, _, { terminalManager }) => {
    const tokenSource = new CancellationTokenSource()
    const token = tokenSource.token
    const decoder = new TextDecoder()
    const messages = terminalManager.messages()
    ;(async () => {
        while (!token.isCancelled) {
            const result = await Promise.race([
                messages.next(),
                tokenSource.promise(),
            ])

            if (result === undefined || result.done) {
                return
            }

            const message = result.value

            if (message instanceof OpenNewTabReply) {
                dispatch(terminalNewTabCreated(message.id))
            }

            if (message instanceof StdOutMessage) {
                const payload = decoder.decode(Buffer.from(message.data))
                terminalManager.write(message.tabId, payload)
            }
        }
    })()

    return tokenSource
}

export default processTty
