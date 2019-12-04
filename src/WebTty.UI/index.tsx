// Must be the first import
if (process.env.NODE_ENV === "development") {
    // Must use require here as import statements are only allowed
    // to exist at the top of a file.
    require("preact/debug")
}

import { h, render } from "preact"

import "./index.css"
import App from "./containers/App"
import createContext from "./context"

const [store, services] = createContext()

render(<App store={store} services={services} />, document.body)
