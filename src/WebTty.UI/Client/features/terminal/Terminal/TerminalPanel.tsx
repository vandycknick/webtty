import { h, FunctionComponent } from "preact"
import { useCallback } from "preact/hooks"
import { ITheme } from "xterm"
import { useDispatch } from "react-redux"

import terminalManager from "../terminalManager"
import Terminal from "./Terminal"
import { writeStdIn, resizeTab } from "../terminalActions"
import { useDebounce } from "common/hooks/useDebounce"

interface TerminalPanelProps {
    tabId: string
    theme?: ITheme
}

const TerminalPanel: FunctionComponent<TerminalPanelProps> = ({
    tabId,
    theme,
}) => {
    const dispatch = useDispatch()
    const writeInput = useCallback(
        (message: string) => dispatch(writeStdIn(tabId, message)),
        [tabId, dispatch],
    )

    const onResize = useCallback(
        ({ cols, rows }: { cols: number; rows: number }) =>
            dispatch(resizeTab(tabId, cols, rows)),
        [tabId, dispatch],
    )
    const onResizeDebounced = useDebounce(onResize, 300)

    const term = terminalManager.get(tabId)

    if (term === undefined) return null

    return (
        <Terminal
            term={term}
            theme={theme}
            onInput={writeInput}
            onResize={onResizeDebounced}
        />
    )
}

export default TerminalPanel
