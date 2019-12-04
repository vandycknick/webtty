import { h, FunctionalComponent } from "preact"
import { useRef, useEffect } from "preact/hooks"
import { Terminal as Xterm, ITheme, ITerminalAddon, IDisposable } from "xterm"
import "xterm/css/xterm.css"

import "./Terminal.css"

type TerminalProps = {
    dataSource: AsyncIterable<string>
    theme?: ITheme
    addons?: ITerminalAddon[]
    onInput?: (data: string) => void
    onResize?: (data: { cols: number; rows: number }) => void
    onTitle?: (title: string) => void
    onAddonsLoaded?: () => void
    autoBuffer?: boolean
}

type DataSourceOptions = {
    dataSource: AsyncIterable<string>
    terminal: Xterm
    autoBuffer?: boolean
}

const consumeDataSource = async ({
    dataSource,
    terminal,
    autoBuffer = true,
}: DataSourceOptions): Promise<void> => {
    let buffer = ""

    const flushBuffer = (): void => {
        terminal.write(buffer)
        buffer = ""
    }

    const pushToBuffer = (msg: string): void => {
        if (buffer !== "") {
            buffer += msg
        } else {
            buffer = msg
            setTimeout(flushBuffer, 10)
        }
    }

    for await (const message of dataSource) {
        if (autoBuffer) {
            pushToBuffer(message)
        } else {
            terminal.write(message)
        }
    }
}

const Terminal: FunctionalComponent<TerminalProps> = ({
    dataSource,
    onInput,
    onResize,
    onTitle,
    onAddonsLoaded,
    theme,
    addons = [],
    autoBuffer = true,
}: TerminalProps) => {
    const wrapper = useRef<HTMLDivElement>()
    const terminalRef = useRef<Xterm | null>(null)

    useEffect(() => {
        const disposables: IDisposable[] = []
        if (!terminalRef.current) {
            terminalRef.current = new Xterm()
        }

        const terminal = terminalRef.current
        if (wrapper.current) {
            terminal.open(wrapper.current)

            consumeDataSource({ dataSource, terminal, autoBuffer })

            addons.forEach(addon => terminal.loadAddon(addon))

            onAddonsLoaded?.()

            if (onInput) {
                disposables.push(terminal.onData(onInput))
            }

            if (onResize) {
                disposables.push(terminal.onResize(onResize))
            }

            if (onTitle) {
                terminal.onTitleChange(onTitle)
            }

            if (theme) {
                terminal.setOption("theme", theme)
            }

            terminal.focus()
        }

        return (): void => {
            disposables.forEach(disposable => disposable.dispose())
        }
    }, [wrapper.current])

    return (
        <div
            className="terminal-wrapper"
            style={{ backgroundColor: theme?.background || "black" }}
            ref={wrapper}
        ></div>
    )
}

export default Terminal
