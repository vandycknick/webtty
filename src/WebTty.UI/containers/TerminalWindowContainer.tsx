import { useActionCreators } from "@nvd/use-redux"
import { h } from "preact"
import { useEffect } from "preact/hooks";
import { ITerminalAddon } from "xterm";

import Terminal from "../components/Terminal"
import ResizeObserver from "../components/ResizeObserver"
import { startTerminal, getTabStdoutStream, writeStdin, resizeTerminal } from "../store";

type TerminalContainerProps = {
    addons: ITerminalAddon[]
    onResize: () => void
}

const TerminalWindowContainer = ({ addons, onResize }: TerminalContainerProps) => {
    const actions = useActionCreators({ startTerminal } as any)

    useEffect(() => {
        actions.startTerminal();
    }, [addons])

    return (
        <ResizeObserver onChange={onResize}>
            <Terminal
                dataSource={getTabStdoutStream(1)}
                addons={addons}
                onResize={(data) => resizeTerminal(data.cols, data.rows)}
                onInput={writeStdin}
                onTitle={title => document.title = title}
                onAddonsLoaded={onResize}
            />
        </ResizeObserver>
    )
}

export default TerminalWindowContainer;
