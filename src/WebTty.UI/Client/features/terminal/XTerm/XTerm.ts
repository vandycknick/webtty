import { Terminal, ITheme, ITerminalAddon } from "xterm"
import { FitAddon } from "xterm-addon-fit"
import { IDisposable } from "common/types"
import { ITerminal } from "../types"
import detectWebgl2Support from "common/utils/detectWebgl2Support"
import { WebglAddon } from "xterm-addon-webgl"

class XTerm implements ITerminal {
    private readonly _terminal: Terminal
    private _isOpen = false
    private readonly ref: HTMLDivElement
    private readonly fitAddon = new FitAddon()
    private readonly _addons: ITerminalAddon[] = []
    private readonly _writeDelay: number = 10
    private _disposables: IDisposable[] = []

    constructor() {
        this._terminal = new Terminal()
        this.ref = document.createElement("div")
        this.ref.style.width = "100%"
        this.ref.style.height = "100%"
        this._addons = [this.fitAddon]

        if (detectWebgl2Support()) {
            this._addons.push(new WebglAddon())
        }
    }

    public open(parent: HTMLElement): void {
        parent.appendChild(this.ref)

        if (this._isOpen === false) {
            this._terminal.open(this.ref)
            this._addons.forEach(addon => this._terminal.loadAddon(addon))
        }

        this._isOpen = true
    }

    public detach = (): void => {
        this._disposables.forEach(disposable => disposable.dispose())
        const wrapper = this.ref.parentNode
        wrapper?.removeChild(this.ref)
        this._disposables = []
    }

    public resize = (): void => {
        if (this.ref.parentElement) this.fitAddon.fit()
    }

    private _buffer = ""
    private _flush = (): void => {
        this._terminal.write(this._buffer)
        this._buffer = ""
    }

    public write(data: string): void {
        if (this._buffer !== "") {
            this._buffer += data
        } else {
            this._buffer = data
            setTimeout(this._flush, this._writeDelay)
        }
    }

    public focus(): void {
        this._terminal.focus()
    }

    public onData(handler: (data: string) => void): void {
        const disposable = this._terminal.onData(handler)
        this._disposables.push(disposable)
    }

    public onResize(
        handler: (data: { cols: number; rows: number }) => void,
    ): void {
        const disposable = this._terminal.onResize(handler)
        this._disposables.push(disposable)
    }

    public onTitleChange(handler: (title: string) => void): void {
        const disposable = this._terminal.onTitleChange(handler)
        this._disposables.push(disposable)
    }

    public setTheme(theme?: ITheme): void {
        this._terminal.setOption("theme", theme)
    }

    public dispose(): void {
        this.detach()
        this._terminal.dispose()
    }
}

export default XTerm
