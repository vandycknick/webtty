interface ITheme {
    name: string

    foreground?: string
    background: string

    black?: string
    brightBlack?: string

    white?: string
    brightWhite?: string

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

    purple?: string
    brightPurple?: string

    cyan?: string
    brightCyan?: string
}

const defaultTheme: ITheme = {
    name: "default",
    background: "#1e1e1e",
    foreground: "#cccccc",
    red: "#cd3131",
    green: "#0dbc79",
    yellow: "#e5e510",
    blue: "#2472c8",
    cyan: "#11a8cd",
    white: "#e5e5e5",
    brightBlack: "#666666",
    brightRed: "#f14c4c",
    brightGreen: "#23d18b",
    brightYellow: "#f5f543",
    brightBlue: "#3b8eea",
    brightCyan: "#29b8db",
    brightWhite: "#e5e5e5",
}

export { ITheme, defaultTheme }
