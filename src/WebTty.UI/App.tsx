import { h, FunctionComponent } from "preact"
import { useMemo } from "preact/hooks"
import { FitAddon } from "xterm-addon-fit"

import "./index.css"
import { useDebounce } from "./hooks/useDebounce"
import TerminalWindowContainer from "./containers/TerminalWindowContainer"

const App: FunctionComponent = () => {
    const fit = useMemo(() => new FitAddon())
    const debounceFit = useDebounce((): void => fit.fit(), 200)

    return <TerminalWindowContainer addons={[fit]} onResize={debounceFit} />
}

export default App
