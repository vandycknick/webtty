import { Messages } from "./types"
import { IConnection } from "common/connection"
import serializeMessage from "./serializeMessage"

type MessageWriter = (message: Messages) => void

const createMessageWriter = (connection: IConnection): MessageWriter => {
    return (message: Messages): void => {
        const data = serializeMessage(message)
        connection.send(data)
    }
}

export default createMessageWriter
