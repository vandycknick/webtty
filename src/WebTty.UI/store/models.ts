class TerminalNewMessage {
    public type: number = 0

    public serialize() {
        return [this.type]
    }
}

class TerminalInputMessage {
    public type: number = 1
    public id: number
    public payload: string

    constructor(id: number, payload: string) {
        this.id = id;
        this.payload = payload
    }

    public serialize() {
        return [this.type, this.id, this.payload]
    }
}

class TerminalOutputMessage {
    public type: number = 2
    public id: number
    public payload: string

    constructor(id: number, payload: string) {
        this.id = id
        this.payload = payload
    }
}

class TerminalResizeMessage {
    public type: number = 3
    public id: number
    public cols: number
    public rows: number

    constructor (id: number, cols: number, rows: number) {
        this.id = id
        this.cols = cols
        this.rows = rows
    }

    public serialize() {
        return [this.type, this.id, this.cols, this.rows]
    }
}

class TerminalNewTabMessage {
    public type: number = 4

    public serialize() {
        return [this.type]
    }
}

class TerminalNewTabCreatedMessage {
    public type: number = 5
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
    TerminalNewTabCreatedMessage
}
