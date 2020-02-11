import {
    OpenNewTabRequest,
    OpenNewTabReply,
    ResizeTabRequest,
    SendInputRequest,
    OpenOutputRequest,
    OutputEvent,
} from "@webtty/messages"

class UnknownMessage {
    public type: string
    public payload: unknown

    constructor(type: string, payload: unknown) {
        this.type = type
        this.payload = payload
    }

    public toJSON(): { type: string; payload: unknown } {
        return {
            type: this.type,
            payload: this.payload,
        }
    }
}

type Messages =
    | OpenNewTabRequest
    | OpenNewTabReply
    | ResizeTabRequest
    | SendInputRequest
    | OpenOutputRequest
    | OutputEvent
    | UnknownMessage

export { UnknownMessage, Messages }
