import { h, Fragment } from "preact"
import { FitAddon } from "xterm-addon-fit"
import msgpack5 from "msgpack5"

import "./index.css"
import Terminal from "./components/Terminal"
import ResizeObserver from "./components/ResizeObserver"
import fromEmitter, { $terminated } from "./utils/fromEmitter"
import debounce from "./utils/debounce";
import TerminalResizeMessage from "./models/TerminalResizeMessage";
import TerminalInputMessage from "./models/TerminalInputMessage";
import TerminalOutputMessage from "./models/TerminalOutputMessage";

const socket = new WebSocket("ws://localhost:5000/ws")
socket.binaryType = "arraybuffer"

const isSocketReady = new Promise((res, rej) => {
    const isOpen = () => {
        socket.removeEventListener("open", isOpen)
        socket.removeEventListener("error", hasError)
        res()
    };
    const hasError = () => {
        socket.removeEventListener("open", isOpen)
        socket.removeEventListener("error", hasError)
        rej()
    }

    socket.addEventListener("open", isOpen)
    socket.addEventListener("error", hasError)
})
const socketReady = () => isSocketReady;

const dataSource = fromEmitter<MessageEvent>(socket)

async function* decodeMessages(source: AsyncIterable<MessageEvent | typeof $terminated>) {
    const msgpack = msgpack5()
    const decoder = new TextDecoder()

    for await (let message of source) {
        if (message === $terminated) break

        const properties = msgpack.decode(Buffer.from(message.data))
        const body = decoder.decode(properties[1])
        const msg = new TerminalOutputMessage(body);
        yield msg.payload
    }
}

async function writeToSocket(msg: TerminalResizeMessage | TerminalInputMessage) {
    const msgpack = msgpack5()
    if (socket.readyState === socket.CONNECTING) {
        await socketReady();
    }

    if (socket.readyState === socket.OPEN) {
        console.log(msg);
        const payload = msgpack.encode(msg.serialize())
        socket.send(payload.slice())
    }
}

const fit = new FitAddon()
const debounceFit = debounce(() => fit.fit(), 200)

const App = () => (
    <Fragment>
        <ResizeObserver onChange={debounceFit}>
            <Terminal
                dataSource={decodeMessages(dataSource)}
                addons={[fit]}
                onResize={(data) => writeToSocket(new TerminalResizeMessage(data.cols, data.rows))}
                onInput={input => writeToSocket(new TerminalInputMessage(input))}
                onTitle={title => document.title = title}
                onAddonsLoaded={debounceFit}
            />
        </ResizeObserver>
    </Fragment>
)

export default App;
