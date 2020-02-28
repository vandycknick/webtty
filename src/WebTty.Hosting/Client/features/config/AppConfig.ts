import { ITheme } from "features/theme"

interface AppConfig {
    ptyHost: string
    ptyPath: string
    selectedTheme: string
    themes?: ITheme[]
}

export { AppConfig }
