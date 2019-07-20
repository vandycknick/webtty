import { h, FunctionComponent } from "preact"
import { ITerminalAddon } from "xterm"
import Terminal from "../components/Terminal"
import ResizeObserver from "../components/ResizeObserver"
import useWebTty from "../services/useWebTty"
import configBuilder from "../config"

type TerminalContainerProps = {
    addons: ITerminalAddon[]
    onResize: () => void
}

const setDocumentTitle = (title: string): void => {
    document.title = title
}

const config = configBuilder()

const TerminalWindowContainer: FunctionComponent<TerminalContainerProps> = ({
    addons,
    onResize,
}: TerminalContainerProps) => {
    const { state, resizeTerminal, writeStdIn, stdOut } = useWebTty(config.socketUrl)

    if (!state.tabId) return <div>Loading ...</div>

    return (
        <ResizeObserver onChange={onResize}>
            <Terminal
                dataSource={stdOut(state.tabId)}
                addons={addons}
                onResize={(data): void => resizeTerminal(state.tabId as number, data.cols, data.rows)}
                onInput={(msg): void => writeStdIn(state.tabId as number, msg)}
                onTitle={setDocumentTitle}
                onAddonsLoaded={onResize}
            />
        </ResizeObserver>
    )
}

export default TerminalWindowContainer
