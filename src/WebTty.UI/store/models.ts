class TerminalNewMessage {
    public type: number = 0

    public serialize() {
        return [this.type]
    }
}

class TerminalInputMessage {
    public type: number = 1
    public payload: string

    constructor(payload: string) {
        this.payload = payload
    }

    public serialize() {
        return [this.type, this.payload]
    }
}

class TerminalOutputMessage {
    public type: number = 2
    public payload: string

    constructor(payload: string) {
        this.payload = payload
    }
}

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

class TerminalNewTabMessage {
    public type: number = 4
    public id: number

    constructor (id: number) {
        this.id = id
    }
}

export {
    TerminalNewMessage,
    TerminalInputMessage,
    TerminalOutputMessage,
    TerminalResizeMessage,
    TerminalNewTabMessage,
}
