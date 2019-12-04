import { h, FunctionComponent } from "preact"
import { Provider } from "react-redux"
import { Store } from "redux"
import { AppServices } from "application"
import WindowContainer from "./WindowContainer"
import { ServiceProvider } from "lib/hooks/useServices"

interface AppProps {
    store: Store
    services: AppServices
}

const App: FunctionComponent<AppProps> = ({ store, services }) => {
    return (
        <ServiceProvider value={services}>
            <Provider store={store}>
                <WindowContainer />
            </Provider>
        </ServiceProvider>
    )
}

export default App
