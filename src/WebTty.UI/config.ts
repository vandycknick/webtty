type AppConfig = {
    socketUrl: string
}

const configBuilder = (): AppConfig => {
    switch (process.env.NODE_ENV) {
        case "development":
            return {
                socketUrl: "ws://localhost:5000/ws",
            }

        case "production":
        default:
            return {
                socketUrl: `ws://${window.location.host}/ws`,
            }
    }
}

export { AppConfig }
export default configBuilder
