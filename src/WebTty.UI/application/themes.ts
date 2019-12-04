interface ITheme {
    foreground?: string
    background: string

    black?: string
    brightBlack?: string

    red?: string
    brightRed?: string

    green?: string
    brightGreen?: string

    yellow?: string
    brightYellow?: string

    blue?: string
    brightBlue?: string

    magenta?: string
    brightMagenta?: string

    cyan?: string
    brightCyan?: string

    white?: string
    brightWhite?: string
}

interface IThemeMap {
    default: ITheme
    solarized: ITheme
}

const themes: IThemeMap = {
    default: {
        background: "#1e1e1e",
    },
    solarized: {
        foreground: "#93a1a1",
        background: "#002b36",

        black: "#002b36",
        brightBlack: "#657b83",

        red: "#dc322f",
        brightRed: "#dc322f",

        green: "#859900",
        brightGreen: "#859900",

        yellow: "#b58900",
        brightYellow: "#b58900",

        blue: "#268bd2",
        brightBlue: "#268bd2",

        magenta: "#6c71c4",
        brightMagenta: "#6c71c4",

        cyan: "#2aa198",
        brightCyan: "#2aa198",

        white: "#93a1a1",
        brightWhite: "#fdf6e3",
    },
}

export { ITheme, IThemeMap }
export default themes
