// Must be the first import
if (process.env.NODE_ENV === "development") {
    // Must use require here as import statements are only allowed
    // to exist at the top of a file.
    require("preact/debug")
}

import { h, render } from "preact"

import "./index.css"
import App from "./App"

render(<App />, document.body)
