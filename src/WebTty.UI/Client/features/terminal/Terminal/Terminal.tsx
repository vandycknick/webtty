import { h, FunctionComponent } from "preact"
import { useRef, useEffect, useCallback } from "preact/hooks"
import { ITheme } from "xterm"

import "xterm/css/xterm.css"
import "./Terminal.css"

import { IDisposable } from "common/types"
import ResizeObserver from "common/components/ResizeObserver/ResizeObserver"
import { Term } from "../terminalManager"

type TerminalProps = {
    term: Term
    theme?: ITheme
    onInput?: (data: string) => void
    onResize?: (data: { cols: number; rows: number }) => void
    onTitle?: (title: string) => void
}

const Terminal: FunctionComponent<TerminalProps> = ({
    term: xterm,
    onInput,
    onResize,
    onTitle,
    theme,
}) => {
    const wrapperRef = useRef<HTMLDivElement>()

    const onFit = useCallback(() => xterm.resize(), [xterm])

    useEffect(() => {
        const disposables: IDisposable[] = []
        const wrapper = wrapperRef.current

        if (wrapper == undefined) return

        xterm.open(wrapper)

        if (onInput) {
            disposables.push(xterm.terminal.onData(onInput))
        }

        if (onResize) {
            disposables.push(xterm.terminal.onResize(onResize))
        }

        if (onTitle) {
            xterm.terminal.onTitleChange(onTitle)
        }

        if (theme) {
            xterm.terminal.setOption("theme", theme)
        }

        xterm.terminal.focus()

        return (): void => {
            disposables.forEach(disposable => disposable.dispose())
            xterm.detach()
        }
    }, [xterm, onInput, onResize, onTitle, theme])

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
