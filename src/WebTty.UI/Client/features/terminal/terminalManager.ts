import { Terminal as Xterm } from "xterm"
import { FitAddon } from "xterm-addon-fit"
import { WebglAddon } from "xterm-addon-webgl"

class Term {
    public terminal: Xterm
    public ref: HTMLDivElement
    private readonly fit = new FitAddon()
    private readonly webgl = new WebglAddon()
    private isOpened = false

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
        this.terminal.loadAddon(this.webgl)
    }

    resize(): void {
        this.fit.fit()
    }

    write(data: string): void {
        this.terminal.write(data)
    }
}

const terminalManager = new Map<string, Term>()

export { Term }
export default terminalManager
