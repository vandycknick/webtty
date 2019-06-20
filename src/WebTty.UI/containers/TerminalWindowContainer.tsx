import { useActionCreators, useRedux } from "@nvd/use-redux"
import { h } from "preact"
import { useEffect } from "preact/hooks";
import { ITerminalAddon } from "xterm";

import Terminal from "../components/Terminal"
import ResizeObserver from "../components/ResizeObserver"
import { startTerminal, getTabStdoutStream, writeStdin, resizeTerminal, AppState } from "../store";

type TerminalContainerProps = {
    addons: ITerminalAddon[]
    onResize: () => void
}

const TerminalWindowContainer = ({ addons, onResize }: TerminalContainerProps) => {
    const actions = useActionCreators({ startTerminal } as any)
    const state = useRedux<AppState>();

    useEffect(() => {
        if (!state.terminal.tabId) actions.startTerminal();
    }, [state.terminal.tabId])

    if (!state.terminal.tabId) return <div>Loading ...</div>

    return (
        <ResizeObserver onChange={onResize}>
            <Terminal
                dataSource={getTabStdoutStream(state.terminal.tabId as number)}
                addons={addons}
                onResize={(data) => resizeTerminal(state.terminal.tabId as number, data.cols, data.rows)}
                onInput={(msg: string) => writeStdin(state.terminal.tabId as number, msg)}
                onTitle={title => document.title = title}
                onAddonsLoaded={onResize}
            />
        </ResizeObserver>
    )
}

export default TerminalWindowContainer;
