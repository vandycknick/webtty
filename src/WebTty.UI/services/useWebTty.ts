import { useReducer, useMemo, useEffect } from "preact/hooks"

import useWebSocket from "../hooks/useWebSocket"
import { terminalReducer, stdoutMessageStream } from "./terminal"
import AsyncQueue from "../utils/AsyncQueue"
import { openNewTab, resizeTerminal, writeStdIn, newTabMessageStream } from "./terminal"
import { TerminalState } from "./types"
import { serializeCommands, Commands, Events, deserializeMessages } from "./serializers"

type WebTtyConnection = {
    state: TerminalState
    openNewTab: () => void
    resizeTerminal: (id: string, cols: number, rows: number) => void
    writeStdIn: (id: string, input: string) => void
    stdOut: (id: string) => AsyncIterable<string>
}

const useWebTty = (endpoint: string): WebTtyConnection => {
    const [dataStream, sendMessage] = useWebSocket(endpoint, {
        binaryType: "arraybuffer",
    })
    const [state, dispatch] = useReducer(terminalReducer, { tabId: undefined })

    const actions = useMemo(() => {
        const dispatchCommand = (command: Commands): void => {
            const serialized = serializeCommands(command)
            sendMessage(serialized)
        }

        return {
            openNewTab: openNewTab(dispatchCommand),
            resizeTerminal: resizeTerminal(dispatchCommand),
            writeStdIn: writeStdIn(dispatchCommand),
        }
    }, [sendMessage])

    const messageStream = useMemo((): AsyncQueue<Events> => {
        const eventStream = deserializeMessages(dataStream)
        const queue = AsyncQueue.from(eventStream)
        return queue
    }, [dataStream])

    useEffect(() => {
        if (!state.tabId) actions.openNewTab()

        const newTabStream = AsyncQueue.from(messageStream)
        newTabMessageStream(newTabStream)(dispatch)

        return (): void => messageStream.dispose()
    }, [dataStream])

    const getStdout = (id: string): AsyncIterable<string> => stdoutMessageStream(id, messageStream)

    return {
        state,
        ...actions,
        stdOut: getStdout,
    }
}

export default useWebTty
