import { ThunkAction } from "redux-thunk"

import { IDisposable } from "lib/types"
import Deferred from "lib/utils/Deferred"

import { TerminalActions } from "../actions"
import { AppServices } from "../interfaces"
import { OpenNewTabReply, StdOutMessage } from "@webtty/messages"
import terminalNewTabCreated from "application/actionCreators/terminalNewTabCreated"

class CancellationToken {
    private readonly deferred: Deferred<void>
    private isCancelledInternal = false

    constructor() {
        this.deferred = new Deferred()
        this.deferred.then(() => (this.isCancelledInternal = true))
    }

    public get isCancelled(): boolean {
        return this.isCancelledInternal
    }

    public cancel = (): void => this.deferred.resolve()

    public promise = (): Promise<void> => this.deferred
}

const processTty = (): ThunkAction<
    IDisposable,
    null,
    AppServices,
    TerminalActions
> => (dispatch, _, { terminalManager }) => {
    const token = new CancellationToken()
    const decoder = new TextDecoder()
    const messages = terminalManager.messages()
    ;(async () => {
        while (!token.isCancelled) {
            const result = await Promise.race([
                messages.next(),
                token.promise(),
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

    return {
        dispose: (): void => token.cancel(),
    }
}

export default processTty
