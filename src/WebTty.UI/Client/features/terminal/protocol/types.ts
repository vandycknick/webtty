import {
    StdInputRequest,
    ResizeTabMessage,
    OpenNewTabReply,
    OpenNewTabRequest,
    StdOutMessage,
} from "@webtty/messages"

class UnknownMessage {
    public type: string
    public payload: unknown

    constructor(type: string, payload: unknown) {
        this.type = type
        this.payload = payload
    }

    public toJSON(): unknown {
        return {}
    }
}

type Messages =
    | StdOutMessage
    | OpenNewTabRequest
    | OpenNewTabReply
    | ResizeTabMessage
    | StdInputRequest
    | UnknownMessage

export { UnknownMessage, Messages }
