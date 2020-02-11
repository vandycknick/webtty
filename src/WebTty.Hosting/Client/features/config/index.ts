import { AppConfig } from "./AppConfig"

const configValidator = (config: Partial<AppConfig>): config is AppConfig => {
    if (config.ptyHost === undefined) {
        throw new Error("AppConfig ttyHost is not defined")
    }

    if (config.ptyPath === undefined) {
        throw new Error("AppConfig ttyPath is not defined")
    }

    if (config.theme === undefined) {
        throw new Error("AppConfig theme is not defined")
    }

    return true
}

export { configValidator, AppConfig }
