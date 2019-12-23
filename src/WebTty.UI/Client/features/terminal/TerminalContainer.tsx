import { h, FunctionComponent, Fragment } from "preact"
import { useSelector } from "react-redux"
import { ITheme } from "xterm"

import { getSelectedTab } from "./terminalSelectors"
import TerminalPanel from "./Terminal/TerminalPanel"
import { getCurrentTheme } from "features/theme/themeSelectors"

interface TerminalContainerProps {
    theme?: ITheme
}

const TerminalContainer: FunctionComponent<TerminalContainerProps> = () => {
    const selectedTab = useSelector(getSelectedTab)
    const theme = useSelector(getCurrentTheme)
    return (
        <Fragment>
            <TerminalPanel tabId={selectedTab} theme={theme} />
        </Fragment>
    )
}

export default TerminalContainer
