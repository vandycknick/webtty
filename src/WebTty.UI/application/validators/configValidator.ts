import { AppConfig } from "../interfaces"

const configValidator = (config: Partial<AppConfig>): config is AppConfig => {
    if (config.socketUrl === undefined) {
        throw new Error("AppConfig socketUrl is not defined")
    }

    return true
}

export default configValidator
