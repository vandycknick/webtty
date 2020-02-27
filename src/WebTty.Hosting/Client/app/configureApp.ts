import configureStore from "app/configureStore"
import ConfigBuilder from "common/utils/ConfigBuilder"
import { ConnectionBuilder } from "common/connection"
import { AppConfig, configValidator } from "features/config"
import createTerminalMiddleware from "features/terminal/createTerminalMiddleware"
import { createThemeState } from "features/theme"

const configureApp = (): ReturnType<typeof configureStore> => {
    const config = new ConfigBuilder<AppConfig>()
        .addVariable("ptyHost", `ws://${window.location.host}`)
        .addVariable("selectedTheme", "default")
        .addFromDom("config")
        .addVariableDevelopment("ptyHost", `ws://localhost:5000`)
        .addVariableDevelopment("ptyPath", "/pty")
        .build(configValidator)

    const connection = new ConnectionBuilder()
        .withUrl(`${config.ptyHost}${config.ptyPath}`)
        .useWebSocket("arraybuffer")
        .build()

    const terminal = createTerminalMiddleware({
        connection,
    })

    const store = configureStore({
        middleware: [terminal],
        state: {
            ...createThemeState(config.selectedTheme, config.themes ?? []),
        },
    })
    return store
}

export default configureApp
