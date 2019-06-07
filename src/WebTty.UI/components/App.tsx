import { h, Fragment, ComponentChildren } from "preact";

type AppProps = {
    children: ComponentChildren
}

const App = ({ children }: AppProps) => (
    <Fragment>
        {children}
    </Fragment>
)

export default App;
