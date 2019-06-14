class TerminalResizeMessage {
    public type: number = 3
    public cols: number
    public rows: number

    constructor (cols: number, rows: number) {
        this.cols = cols
        this.rows = rows
    }

    public serialize() {
        return [this.type, this.cols, this.rows]
    }
}

export default TerminalResizeMessage
