import WebSocketConnection from "./WebSocketConnection"
import IConnection from "./IConnection"

type ConnectionFactory = (url: string) => IConnection

class ConnectionBuilder {
    private url = ""
    private connectionFactory: ConnectionFactory | undefined = undefined

    public withUrl(url: string): this {
        this.url = url
        return this
    }

    public useWebSocket(binaryType: BinaryType): this {
        this.connectionFactory = (url: string) =>
            new WebSocketConnection(url, binaryType)
        return this
    }

    public build(): IConnection {
        if (this.connectionFactory === undefined) {
            throw new Error("No default connection provided")
        }

        return this.connectionFactory(this.url)
    }
}

export default ConnectionBuilder
