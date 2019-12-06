import { h, FunctionComponent } from "preact"
import { useDispatch, useSelector } from "react-redux"
import { AppDispatch, writeInput, resizeTab, AppServices } from "application"
import { useCallback, useMemo } from "preact/hooks"
import Tab from "components/Tab/Tab"
import useServices from "lib/hooks/useServices"
import selectTheme from "application/selectors/selectTheme"

interface TabContainerProps {
    tabId: string
}

const TabContainer: FunctionComponent<TabContainerProps> = ({ tabId }) => {
    const dispatch = useDispatch<AppDispatch>()
    const theme = useSelector(selectTheme)
    const { terminalManager } = useServices<AppServices>()
    const stdout = useMemo(() => terminalManager.getStdout(tabId), [
        terminalManager,
        tabId,
    ])

    const onInput = useCallback(
        (payload: string) => dispatch(writeInput(tabId, payload)),
        [tabId, dispatch],
    )
    const onResize = useCallback(
        (data: { cols: number; rows: number }) =>
            dispatch(resizeTab(tabId, data.cols, data.rows)),
        [tabId, dispatch],
    )

    return (
        <Tab
            source={stdout}
            theme={theme}
            onInput={onInput}
            onResize={onResize}
        />
    )
}

export default TabContainer
