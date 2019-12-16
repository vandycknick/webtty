import { h, FunctionalComponent } from "preact"
import { useRef, useEffect } from "preact/hooks"
import { Terminal as Xterm, ITheme, ITerminalAddon, IDisposable } from "xterm"

import "xterm/css/xterm.css"
import "./Terminal.css"

import { CancellationTokenSource } from "lib/utils/CancellationToken"

type TerminalProps = {
    dataSource: AsyncIterableIterator<string>
    theme?: ITheme
    addons?: ITerminalAddon[]
    onInput?: (data: string) => void
    onResize?: (data: { cols: number; rows: number }) => void
    onTitle?: (title: string) => void
    onAddonsLoaded?: () => void
    autoBuffer?: boolean
}

type DataSourceOptions = {
    dataSource: AsyncIterableIterator<string>
    terminal: Xterm
    autoBuffer?: boolean
}

const consumeDataSource = ({
    dataSource,
    terminal,
    autoBuffer = true,
}: DataSourceOptions): IDisposable => {
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

    const tokenSource = new CancellationTokenSource()
    const { token } = tokenSource
    ;(async function() {
        while (!token.isCancelled) {
            const data = await dataSource.next()

            if (data.done || token.isCancelled) return

            if (autoBuffer) {
                pushToBuffer(data.value)
            } else {
                terminal.write(data.value)
            }
        }
        console.log("exiting")
    })()

    return tokenSource
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

    // Only runs on mount
    useEffect(() => {
        const disposables: IDisposable[] = []
        let terminal: Xterm | undefined = undefined

        if (wrapper.current) {
            terminal = new Xterm()
            terminal.open(wrapper.current)

            disposables.push(
                consumeDataSource({ dataSource, terminal, autoBuffer }),
            )

            addons.forEach(addon => terminal?.loadAddon(addon))

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
            console.log("unmounting terminal component")
            disposables.forEach(disposable => disposable.dispose())
            terminal?.dispose()
        }
    })

    return (
        <div
            className="terminal-wrapper"
            style={{ backgroundColor: theme?.background || "black" }}
            ref={wrapper}
        ></div>
    )
}

export default Terminal
