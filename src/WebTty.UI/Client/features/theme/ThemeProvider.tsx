import { h, FunctionComponent } from "preact"
import { useSelector } from "react-redux"
import { getCurrentTheme } from "./themeSelectors"

const ThemeProvider: FunctionComponent = ({ children }) => {
    const theme = useSelector(getCurrentTheme)

    return <div style={{ backgroundColor: theme.background }}>{children}</div>
}

export default ThemeProvider
