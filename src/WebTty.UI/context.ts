import { Store } from "redux"
import { AppServices, AppConfig, reducers } from "application"
import ConnectionBuilder from "lib/connection/ConnectionBuilder"
import StoreBuilder from "lib/storeBuilder"
import ConfigBuilder from "lib/utils/ConfigBuilder"

import TerminalManager from "application/services/TerminalManager"
import startSession from "application/services/startSession"
import configValidator from "application/validators/configValidator"

const createContext = (): [Store, AppServices] => {
    // Services
    const config = new ConfigBuilder<AppConfig>()
        .addVariable("socketUrl", `ws://${window.location.host}/tty`)
        .addVariableDevelopment("socketUrl", "ws://localhost:5000/tty")
        .addFromDom("config")
        .build(configValidator)

    const connection = new ConnectionBuilder()
        .withUrl(config.socketUrl)
        .useWebSocket("arraybuffer")
        .build()

    const terminalManager = new TerminalManager(connection)

    const services: AppServices = {
        connection,
        terminalManager,
        config,
    }

    // Store
    const store = new StoreBuilder()
        .useThunkMiddleware(services)
        .useDevTools(true)
        .useReducer(reducers)
        .build()

    // Start Application (TODO: maybe add this as an init function adn export)
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    store.dispatch(startSession() as any)

    return [store, services]
}

export default createContext
