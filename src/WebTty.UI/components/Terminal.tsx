import { h } from "preact"
import { useRef, useEffect } from "preact/hooks"
import { Terminal as Xterm, ITheme, ITerminalAddon } from "xterm"
import "xterm/dist/xterm.css"

import "./Terminal.css"

type TerminalProps = {
    dataSource: AsyncIterable<string>
    addons?: ITerminalAddon[]
    onInput?: (data: string) => void
    onResize?: (data: { cols: number; rows: number; }) => void
    onTitle?: (title: string) => void
    onAddonsLoaded?: () => void
    autoBuffer?: boolean
}

const consumeDataSource = async (dataSource: AsyncIterable<string>, terminal: Xterm, autoBuffer: boolean = true) => {
    let buffer = "";

    const flushBuffer = () => {
        terminal.write(buffer)
        buffer = ""
    }

    const pushToBuffer = (msg: string) => {
        if (buffer !== "") {
            buffer += msg
        } else {
            buffer = msg
            setTimeout(flushBuffer, 16)
        }
    }

    for await (let message of dataSource) {
        if (autoBuffer) {
            pushToBuffer(message)
        } else {
            terminal.write(message);
        }
    }
}

const solarized: ITheme = {
    foreground: "#93a1a1",
    background: "#002b36",
    cursor: "#93a1a1",

    black: "#002b36",
    brightBlack: "#657b83",

    red: "#dc322f",
    brightRed: "#dc322f",

    green: "#859900",
    brightGreen: "#859900",

    yellow: "#b58900",
    brightYellow: "#b58900",

    blue: "#268bd2",
    brightBlue: "#268bd2",

    magenta: "#6c71c4",
    brightMagenta: "#6c71c4",

    cyan: "#2aa198",
    brightCyan: "#2aa198",

    white: "#93a1a1",
    brightWhite: "#fdf6e3",
}

function Terminal({ dataSource, onInput, onResize, onTitle, onAddonsLoaded, addons = [], autoBuffer = true }: TerminalProps) {
    const wrapper = useRef<HTMLDivElement>()
    const terminalRef = useRef<Xterm | null>(null)

    useEffect(() => {
        if (!terminalRef.current) {
            terminalRef.current = new Xterm()
        }

        const terminal = terminalRef.current
        if (wrapper.current) {
            terminal.open(wrapper.current)

            consumeDataSource(dataSource, terminal, autoBuffer);

            addons.forEach(addon => terminal.loadAddon(addon))

            onAddonsLoaded && onAddonsLoaded()

            onInput && terminal.on("data", onInput)
            onResize && terminal.on("resize", onResize)
            onTitle && terminal.on("title", onTitle)

            terminal.setOption("theme", solarized);
            terminal.focus();
        }

        return () => {
            onInput && terminal.off("data", onInput)
            onResize && terminal.off("resize", onResize)
            onTitle && terminal.off("title", onTitle)
        }
    }, [wrapper.current])

    return (
        <div class="terminal-wrapper" style={{ backgroundColor: solarized.background }} ref={wrapper}></div>
    )
}

export default Terminal
