import { FunctionComponent, h } from "preact"
// import { ITerminalAddon } from "xterm"

import { FitAddon } from "xterm-addon-fit"
import { WebglAddon } from "xterm-addon-webgl"
import Terminal from "components/Terminal/Terminal"
import ResizeObserver from "components/ResizeObserver/ResizeObserver"
import { useDebounce } from "lib/hooks/useDebounce"
import { ITheme } from "application/themes"

interface TabProps {
    source: AsyncIterable<string>
    theme?: ITheme
    onInput: (msg: string) => void
    onResize: (data: { cols: number; rows: number }) => void
}

const fit = new FitAddon()
const webgl = new WebglAddon()
const addons = [fit, webgl]

const setDocumentTitle = (title: string): void => {
    document.title = title
}

const Tab: FunctionComponent<TabProps> = ({
    source,
    theme,
    onInput,
    onResize,
}) => {
    const debounceFit = useDebounce((): void => fit.fit(), 200)

    return (
        <ResizeObserver onChange={debounceFit}>
            <Terminal
                dataSource={source}
                theme={theme}
                addons={addons}
                onResize={onResize}
                onInput={onInput}
                onTitle={setDocumentTitle}
                onAddonsLoaded={debounceFit}
            />
        </ResizeObserver>
    )
}

export default Tab
