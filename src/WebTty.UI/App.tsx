import { h, Fragment } from "preact"
import { FitAddon } from "xterm-addon-fit"
import msgpack5 from "msgpack5"

import "./index.css"
import Terminal from "./components/Terminal"
import ResizeObserver from "./components/ResizeObserver"
import fromEmitter, { $terminated } from "./lib/fromEmitter"

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
        yield body
    }
}

async function writeToSocket(data: string) {
    if (socket.readyState === socket.CONNECTING) {
        await socketReady();
    }

    if (socket.readyState === socket.OPEN) {
        socket.send(data);
    }
}

const debounce = (callback: any, time: number) => {
    let interval: any;
    return (...args: any[]) => {
        clearTimeout(interval);
        interval = setTimeout(() => {
            interval = null;
            callback(...args);
        }, time);
    };
};

const fit = new FitAddon()
const debouncedFit = debounce(() => fit.fit(), 250)

const App = () => (
    <Fragment>
        <ResizeObserver onChange={debouncedFit}>
            <Terminal
                dataSource={decodeMessages(dataSource)}
                addons={[fit]}
                onResize={(data) => console.log(data)}
                onInput={writeToSocket}
                onTitle={title => document.title = title}
                onAddonsLoaded={debouncedFit}
            />
        </ResizeObserver>
    </Fragment>
)

export default App;
