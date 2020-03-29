import { Middleware } from "redux"
import { OpenNewTabReply, OutputEvent } from "@webtty/messages"

import { CancellationTokenSource } from "common/async/CancellationToken"
import { IConnection } from "common/connection"
import { consume } from "common/async/utils"
import {
    TerminalActions,
    TERMINAL_SEND_MESSAGE,
    setStatus,
    openNewTab,
    newTab,
    openStdout,
} from "features/terminal/terminalActions"
import {
    Messages,
    createMessageReader,
    createMessageWriter,
} from "features/terminal/protocol"
import { termManager } from "features/terminal/XTerm"

async function* mapMessagesToActions(
    messageStream: AsyncIterableIterator<Messages>,
): AsyncIterableIterator<TerminalActions> {
    const decoder = new TextDecoder()

    for await (const message of messageStream) {
        if (message instanceof OpenNewTabReply) {
            yield newTab(message.id)
            yield openStdout(message.id)
        }

        if (message instanceof OutputEvent) {
            const payload = decoder.decode(Buffer.from(message.data))
            termManager.write(message.tabId, payload)
            continue
        }
    }
}

interface TerminalMiddlewareOptions {
    connection: IConnection
}

const createTerminalMiddleware = (
    options: TerminalMiddlewareOptions,
): Middleware => {
    const { connection } = options
    const tokenSource = new CancellationTokenSource()
    const read = createMessageReader(connection)
    const write = createMessageWriter(connection)

    return (store) => {
        connection.start().then(() => {
            store.dispatch(setStatus("connected"))
            store.dispatch(openNewTab())

            consume(
                mapMessagesToActions(read()),
                store.dispatch,
                tokenSource.token,
            )
        })

        return (next) => (action: TerminalActions) => {
            if (action.type == TERMINAL_SEND_MESSAGE) {
                write(action.payload)
            }
            return next(action)
        }
    }
}

export { mapMessagesToActions }
export default createTerminalMiddleware
