import { h, FunctionComponent, Fragment } from "preact"
import { useCallback } from "preact/hooks"
import { useSelector, useDispatch } from "react-redux"
import { ITheme } from "xterm"

import { useDebounce } from "common/hooks/useDebounce"
import { getCurrentTheme } from "features/theme/themeSelectors"
import { getSelectedTab } from "./terminalSelectors"
import { termManager } from "./XTerm"
import { resizeTab, writeStdIn } from "./terminalActions"
import Terminal from "./Terminal/Terminal"

interface TerminalContainerProps {
    theme?: ITheme
}

const TerminalContainer: FunctionComponent<TerminalContainerProps> = () => {
    const selectedTab = useSelector(getSelectedTab)
    const theme = useSelector(getCurrentTheme)
    const dispatch = useDispatch()
    const writeInput = useCallback(
        (message: string) => dispatch(writeStdIn(selectedTab, message)),
        [selectedTab, dispatch],
    )

    const onResize = useCallback(
        ({ cols, rows }: { cols: number; rows: number }) => {
            if (selectedTab !== "") {
                dispatch(resizeTab(selectedTab, cols, rows))
            }
        },
        [selectedTab, dispatch],
    )
    const onResizeDebounced = useDebounce(onResize, 300)
    return (
        <Fragment>
            <Terminal
                terminal={termManager.get(selectedTab)}
                theme={theme}
                onInput={writeInput}
                onResize={onResizeDebounced}
            />
        </Fragment>
    )
}

export default TerminalContainer
