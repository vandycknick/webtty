class TerminalOutputMessage {
    public type: number = 2
    public payload: string

    constructor(payload: string) {
        this.payload = payload
    }
}

export default TerminalOutputMessage
