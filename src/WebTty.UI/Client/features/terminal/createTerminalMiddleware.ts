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
} from "./terminalActions"
import terminalManager, { Term } from "./terminalManager"
import { Messages } from "./protocol/types"
import createMessageReader from "./protocol/createMessageReader"
import createMessageWriter from "./protocol/createMessageWriter"

async function* mapMessageToAction(
    messageStream: AsyncIterableIterator<Messages>,
): AsyncIterableIterator<TerminalActions> {
    const decoder = new TextDecoder()

    for await (const message of messageStream) {
        if (message instanceof OpenNewTabReply) {
            terminalManager.set(message.id, new Term())
            yield newTab(message.id)
            yield openStdout(message.id)
        }

        if (message instanceof OutputEvent) {
            const payload = decoder.decode(Buffer.from(message.data))
            const terminal = terminalManager.get(message.tabId)
            if (terminal) {
                terminal.write(payload)
            }

            continue
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
    const read = createMessageReader(connection)
    const write = createMessageWriter(connection)

    return store => {
        if (options.connect) {
            connection.start().then(() => {
                store.dispatch(setStatus("connected"))
                store.dispatch(openNewTab())

                consume(
                    mapMessageToAction(read()),
                    store.dispatch,
                    tokenSource.token,
                )
            })
        }

        return next => (action: TerminalActions) => {
            if (action.type == TERMINAL_SEND_MESSAGE) {
                write(action.payload)
            }
            return next(action)
        }
    }
}

export { mapMessageToAction }
export default createTerminalMiddleware
