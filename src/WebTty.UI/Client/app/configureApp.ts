import configureStore from "app/configureStore"
import ConfigBuilder from "common/utils/ConfigBuilder"
import { ConnectionBuilder } from "common/connection"
import { AppConfig, configValidator } from "features/config"
import createTerminalMiddleware from "features/terminal/createTerminalMiddleware"
import themes from "features/theme/themes"

const configureApp = (): ReturnType<typeof configureStore> => {
    const config = new ConfigBuilder<AppConfig>()
        .addFromDom("config")
        .addVariable("ttyHost", `ws://${window.location.host}`)
        .addVariableDevelopment("ttyHost", `ws://localhost:5000`)
        .addVariableDevelopment("ttyPath", "/pty")
        .addVariableDevelopment("theme", "default")
        .build(configValidator)

    const connection = new ConnectionBuilder()
        .withUrl(`${config.ttyHost}${config.ttyPath}`)
        .useWebSocket("arraybuffer")
        .build()

    const terminal = createTerminalMiddleware({
        connect: true,
        connection,
    })

    const store = configureStore({
        middleware: [terminal],
        state: {
            theme: {
                selected: config.theme,
                themes: { ...themes },
            },
        },
    })
    return store
}

export default configureApp
