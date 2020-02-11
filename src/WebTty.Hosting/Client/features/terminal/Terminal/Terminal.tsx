import { h, FunctionComponent } from "preact"
import { useEffect, useCallback, useRef } from "preact/hooks"
import { ITheme } from "xterm"

import "xterm/css/xterm.css"
import "./Terminal.css"

import ResizeObserver from "common/components/ResizeObserver/ResizeObserver"
import { ITerminal } from "../types"

type TerminalProps = {
    terminal: ITerminal
    theme?: ITheme
    onInput?: (data: string) => void
    onResize?: (data: { cols: number; rows: number }) => void
    onTitle?: (title: string) => void
}

// eslint-disable-next-line @typescript-eslint/no-empty-function
const noop = (): void => {}

const Terminal: FunctionComponent<TerminalProps> = ({
    terminal,
    onInput = noop,
    onResize = noop,
    onTitle = noop,
    theme,
}) => {
    const wrapperRef = useRef<HTMLDivElement>()
    const onFit = useCallback(terminal.resize, [terminal])

    useEffect(() => {
        const wrapper = wrapperRef.current

        if (wrapper == undefined) return

        terminal.open(wrapper)

        terminal.onData(onInput)
        terminal.onResize(onResize)
        terminal.onTitleChange(onTitle)
        terminal.setTheme(theme)

        terminal.focus()

        return terminal.detach
    }, [onInput, onResize, onTitle, terminal, theme])

    return (
        <ResizeObserver onChange={onFit}>
            <div
                className="terminal-wrapper"
                style={{ backgroundColor: theme?.background || "black" }}
                ref={wrapperRef}
            />
        </ResizeObserver>
    )
}

export default Terminal
