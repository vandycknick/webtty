import { h, render } from "preact"
import { FitAddon } from "xterm-addon-fit"
import msgpack5 from "msgpack5"

import "./index.css"
import App from "./components/App"
import Terminal from "./components/Terminal"
import fromEmitter, { $terminated } from "./lib/fromEmitter"

const socket = new WebSocket("ws://localhost:5000/ws")
socket.binaryType = "arraybuffer"

const dataSource = fromEmitter<MessageEvent>(socket)

async function* decodeMessages(source: AsyncIterable<MessageEvent | typeof $terminated>) {
    const msgpack = msgpack5()
    const decoder = new TextDecoder()

    for await (let message of source) {
        if (message === $terminated) break

        const properties = msgpack.decode(Buffer.from(message.data))
        const body = decoder.decode(properties[1])
        yield body;
    }
}

async function writeToSocket(data: string) {
    if (socket.readyState === socket.CONNECTING) {
        await new Promise((res, rej) => {
            const isOpen = () => {
                socket.removeEventListener('open', isOpen)
                res
            };
            const hasError = () => {
                socket.removeEventListener("error", hasError)
                rej();
            }

            socket.addEventListener("open", isOpen);
            socket.addEventListener("error", hasError);
        })
    }

    if (socket.readyState === socket.OPEN) {
        socket.send(data);
    }
}

const fit = new FitAddon();

render(
    <App>
        <Terminal
            dataSource={decodeMessages(dataSource)}
            addons={[ fit ]}
            onResize={() => console.log('resize')}
            onInput={writeToSocket}
            onTitle={title => document.title = title}
            onAddonsLoaded={() => fit.fit()}
        />
    </App>,
    document.body,
);
