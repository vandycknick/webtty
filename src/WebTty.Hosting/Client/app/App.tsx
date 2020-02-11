import { h, FunctionComponent } from "preact"
import TerminalContainer from "features/terminal/TerminalContainer"
import ThemeProvider from "features/theme/ThemeProvider"

const App: FunctionComponent = () => {
    return (
        <ThemeProvider>
            <TerminalContainer />
        </ThemeProvider>
    )
}

export default App
