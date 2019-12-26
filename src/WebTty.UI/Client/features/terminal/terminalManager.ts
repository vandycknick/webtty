import { Terminal as Xterm } from "xterm"
import { FitAddon } from "xterm-addon-fit"
import { WebglAddon } from "xterm-addon-webgl"
import detectWebgl2Support from "common/utils/detectWebgl2Support"

class Term {
    private static hasWebgl2Support: boolean = detectWebgl2Support()

    public terminal: Xterm
    public ref: HTMLDivElement
    private isOpened = false
    private readonly fit = new FitAddon()
    private readonly webgl = new WebglAddon()
    private readonly _writeDelay: number = 10

    constructor() {
        this.terminal = new Xterm()
        this.ref = document.createElement("div")
        this.ref.style.width = "100%"
        this.ref.style.height = "100%"
    }

    public open(): void {
        if (this.isOpened === false) {
            this.terminal.open(this.ref)
        }
        this.isOpened = true
    }

    public loadAddons(): void {
        this.terminal.loadAddon(this.fit)

        if (Term.hasWebgl2Support) {
            this.terminal.loadAddon(this.webgl)
        }
    }

    public resize(): void {
        this.fit.fit()
    }

    private _buffer = ""
    private _flush = (): void => {
        this.terminal.write(this._buffer)
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
}

const terminalManager = new Map<string, Term>()

export { Term }
export default terminalManager
