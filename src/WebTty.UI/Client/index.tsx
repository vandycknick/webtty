import "common/utils/preact-debug"
import { h, render } from "preact"
import { Provider } from "react-redux"
import "./index.css"
import configureApp from "app/configureApp"

const store = configureApp()

const renderApp = (): void => {
    const App = require("./app/App").default
    render(
        <Provider store={store}>
            <App />
        </Provider>,
        document.body,
    )
}

// Support hot reloading of components.
// Whenever the App component file or one of its dependencies
// is changed, re-import the updated component and re-render it
if (module.hot) {
    module.hot.accept("./app/App", renderApp)
}

renderApp()
