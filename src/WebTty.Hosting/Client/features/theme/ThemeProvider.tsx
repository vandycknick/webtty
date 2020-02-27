import { h, FunctionComponent } from "preact"
import { useSelector } from "react-redux"
import { getSelectedTheme } from "./themeSelectors"

const ThemeProvider: FunctionComponent = ({ children }) => {
    const theme = useSelector(getSelectedTheme)

    return <div style={{ backgroundColor: theme.background }}>{children}</div>
}

export default ThemeProvider
