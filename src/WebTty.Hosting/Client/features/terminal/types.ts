import { ITheme } from "xterm"

interface ITerminal {
    open(parent: HTMLElement): void
    detach(): void
    resize(): void
    write(data: string): void
    focus(): void
    onData(handler: (data: string) => void): void
    onResize(handler: (data: { cols: number; rows: number }) => void): void
    onTitleChange(handler: (title: string) => void): void
    setTheme(theme?: ITheme): void
    dispose(): void
}

interface ITerminalManager {
    get(id: string): ITerminal
    write(id: string, data: string): void
}

export { ITerminal, ITerminalManager }
