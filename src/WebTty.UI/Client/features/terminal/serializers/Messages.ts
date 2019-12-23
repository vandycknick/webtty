import {
    StdInputRequest,
    ResizeTabMessage,
    OpenNewTabReply,
    OpenNewTabRequest,
    StdOutMessage,
} from "@webtty/messages"

type Messages =
    | StdOutMessage
    | OpenNewTabRequest
    | OpenNewTabReply
    | ResizeTabMessage
    | StdInputRequest

export default Messages
