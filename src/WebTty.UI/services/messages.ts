class TerminalNewMessage {
    public type: number = 0
}

class TerminalInputMessage {
    public type: number = 1
    public id: number
    public payload: string

    public constructor(id: number, payload: string) {
        this.id = id
        this.payload = payload
    }
}

class TerminalOutputMessage {
    public type: number = 2
    public id: number
    public payload: string

    public constructor(id: number, payload: string) {
        this.id = id
        this.payload = payload
    }
}

class TerminalResizeMessage {
    public type: number = 3
    public id: number
    public cols: number
    public rows: number

    public constructor(id: number, cols: number, rows: number) {
        this.id = id
        this.cols = cols
        this.rows = rows
    }
}

class TerminalNewTabMessage {
    public type: number = 4
}

class TerminalNewTabCreatedMessage {
    public type: number = 5
    public id: number

    public constructor(id: number) {
        this.id = id
    }
}

type TerminalMessage =
    | TerminalNewMessage
    | TerminalInputMessage
    | TerminalOutputMessage
    | TerminalResizeMessage
    | TerminalNewTabMessage
    | TerminalNewTabCreatedMessage

export {
    TerminalMessage,
    TerminalNewMessage,
    TerminalInputMessage,
    TerminalOutputMessage,
    TerminalResizeMessage,
    TerminalNewTabMessage,
    TerminalNewTabCreatedMessage,
}
