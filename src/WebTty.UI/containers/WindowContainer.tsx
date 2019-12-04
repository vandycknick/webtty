import { h, FunctionComponent } from "preact"
import { useSelector, useDispatch } from "react-redux"
import { useEffect } from "preact/hooks"

import { selectTerminalState, AppDispatch } from "application"
import WindowComponent from "components/Window/WindowComponent"
import selectTheme from "application/selectors/selectTheme"
import processTty from "application/services/processTty"
import selectTabs from "application/selectors/selectTabs"
import TabContainer from "./TabContainer"

const WindowContainer: FunctionComponent = () => {
    const dispatch = useDispatch<AppDispatch>()
    const state = useSelector(selectTerminalState)
    const theme = useSelector(selectTheme)
    const tabs = useSelector(selectTabs)

    useEffect(() => {
        const disposable = dispatch(processTty())
        return disposable.dispose
    }, [dispatch])

    return (
        <WindowComponent state={state} theme={theme}>
            {tabs.map(tabId => (
                <TabContainer tabId={tabId} key={tabId} />
            ))}
        </WindowComponent>
    )
}

export default WindowContainer
