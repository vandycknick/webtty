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

export default TerminalInputMessage
