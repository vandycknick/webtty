import { Middleware, Dispatch } from "redux"
import { OpenNewTabReply, StdOutMessage } from "@webtty/messages"
import {
    TerminalActions,
    TERMINAL_SEND_MESSAGE,
    setStatus,
    openNewTab,
    newTab,
} from "./terminalActions"
import terminalManager, { Term } from "./terminalManager"
import {
    CancellationTokenSource,
    CancellationToken,
} from "common/CancellationToken"
import { IConnection } from "common/connection"
import { deserializeMessages } from "./serializers/deserializeMessage"
import serializeMessage from "./serializers/serializeMessage"

const decoder = new TextDecoder()

async function consumeMessages(
    connection: IConnection,
    dispatch: Dispatch,
    token: CancellationToken,
): Promise<void> {
    const dataSource = connection[Symbol.asyncIterator]()
    while (!token.isCancelled) {
        const result = await Promise.race([dataSource.next(), token.promise])

        if (result === undefined || result.done) {
            return
        }

        const buffer = result.value
        if (typeof buffer === "string") continue

        for (const message of deserializeMessages(buffer)) {
            if (message === undefined) continue

            if (message instanceof OpenNewTabReply) {
                dispatch(newTab(message.id))
                terminalManager.set(message.id, new Term())
            }

            if (message instanceof StdOutMessage) {
                const payload = decoder.decode(Buffer.from(message.data))
                terminalManager.get(message.tabId)?.write(payload)
            }
        }
    }
}

interface TerminalMiddlewareOptions {
    connect: boolean
    connection: IConnection
}

const createTerminalMiddleware = (
    options: TerminalMiddlewareOptions,
): Middleware => {
    const { connection } = options
    const tokenSource = new CancellationTokenSource()

    return store => {
        if (options.connect) {
            connection.start().then(() => {
                store.dispatch(setStatus("connected"))
                store.dispatch(openNewTab())
            })
            consumeMessages(connection, store.dispatch, tokenSource.token)
        }

        return next => (action: TerminalActions) => {
            if (action.type == TERMINAL_SEND_MESSAGE) {
                const data = serializeMessage(action.payload)
                connection.send(data)
            }
            return next(action)
        }
    }
}

export default createTerminalMiddleware
