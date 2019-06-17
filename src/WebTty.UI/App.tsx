import { Provider } from "@nvd/use-redux"
import { h } from "preact"
import { FitAddon } from "xterm-addon-fit"

import "./index.css"
import TerminalWindowContainer from "./containers/TerminalWindowContainer"
import debounce from "./utils/debounce";
import { storeBuilder } from "./store";

const fit = new FitAddon()
const debounceFit = debounce(() => fit.fit(), 200)
const store = storeBuilder()

const App = () => (
    <Provider value={store} >
        <TerminalWindowContainer
            addons={[fit]}
            onResize={debounceFit}
        />
    </Provider >
)

export default App;
