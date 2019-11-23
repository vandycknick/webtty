type AppConfig = {
    socketUrl: string
}

const configBuilder = (): AppConfig => {
    switch (process.env.NODE_ENV) {
        case "development":
            return {
                socketUrl: "ws://localhost:5000/tty",
            }

        case "production":
        default:
            return {
                socketUrl: `ws://${window.location.host}/tty`,
            }
    }
}

export { AppConfig }
export default configBuilder
