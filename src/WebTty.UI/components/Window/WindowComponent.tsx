import { h, FunctionComponent } from "preact"
import { ITheme } from "xterm"
import "./styles.css"

interface WindowProps {
    state: string
    theme: ITheme
}

const WindowComponent: FunctionComponent<WindowProps> = ({
    state,
    theme,
    children,
}) => (
    <div
        className="window__wrapper"
        style={{ backgroundColor: theme.background || "black" }}
    >
        {state === "connecting" ? "... Loading" : children}
    </div>
)

export default WindowComponent
