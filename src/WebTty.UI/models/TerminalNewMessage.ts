class TerminalNewMessage {
    public type: number = 0

    public serialize() {
        return [this.type]
    }
}

export default TerminalNewMessage
