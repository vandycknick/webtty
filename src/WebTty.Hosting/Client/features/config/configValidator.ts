import { AppConfig } from "features/config/AppConfig"

const configValidator = (config: Partial<AppConfig>): config is AppConfig => {
    if (config.ptyHost === undefined) {
        throw new Error("AppConfig ptyHost is not defined")
    }

    if (config.ptyPath === undefined) {
        throw new Error("AppConfig ptyPath is not defined")
    }

    if (config.selectedTheme === undefined) {
        throw new Error("AppConfig selectedTheme is not defined")
    }

    return true
}

export { configValidator }
