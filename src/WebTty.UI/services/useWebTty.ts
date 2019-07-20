import { useReducer, useMemo, useEffect } from "preact/hooks"

import useWebSocket from "../hooks/useWebSocket"
import { terminalReducer, stdoutMessageStream } from "./terminal"
import AsyncQueue from "../utils/AsyncQueue"
import { TerminalOutputMessage, TerminalNewTabCreatedMessage, TerminalMessage } from "./messages"
import { openNewTab, resizeTerminal, writeStdIn, parseMessages, newTabMessageStream } from "./terminal"
import { TerminalState } from "./types"
import { serializeTerminalMessage } from "./serializers"

type WebTtyConnection = {
    state: TerminalState
    openNewTab: () => void
    resizeTerminal: (id: number, cols: number, rows: number) => void
    writeStdIn: (id: number, input: string) => void
    stdOut: (id: number) => AsyncIterable<string>
}

const useWebTty = (endpoint: string): WebTtyConnection => {
    const [dataStream, sendMessage] = useWebSocket(endpoint, {
        binaryType: "arraybuffer",
    })
    const [state, dispatch] = useReducer(terminalReducer, { tabId: undefined })

    const actions = useMemo(() => {
        const writeToSocket = (msg: TerminalMessage): void => {
            const serialized = serializeTerminalMessage(msg)
            sendMessage(serialized)
        }

        return {
            openNewTab: openNewTab(writeToSocket),
            resizeTerminal: resizeTerminal(writeToSocket),
            writeStdIn: writeStdIn(writeToSocket),
        }
    }, [sendMessage])

    const messageStream = useMemo((): AsyncQueue<TerminalOutputMessage | TerminalNewTabCreatedMessage> => {
        const messageStream = parseMessages(dataStream)
        const queue = AsyncQueue.from(messageStream)
        return queue
    }, [dataStream])

    useEffect(() => {
        if (!state.tabId) actions.openNewTab()

        const newTabStream = AsyncQueue.from(messageStream)
        newTabMessageStream(newTabStream)(dispatch)

        return (): void => messageStream.dispose()
    }, [dataStream])

    const getStdout = (id: number): AsyncIterable<string> => stdoutMessageStream(id, messageStream)

    return {
        state,
        ...actions,
        stdOut: getStdout,
    }
}

export default useWebTty
