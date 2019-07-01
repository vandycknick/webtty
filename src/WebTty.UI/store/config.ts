import { AppConfig } from "./types";

const configBuilder = (): AppConfig => {

    switch (process.env.NODE_ENV) {
        case "development":
            return {
                socketUrl: "ws://localhost:5000/ws"
            }

        case "production":
        default:
            return {
                socketUrl: `ws://${window.location.host}/ws`
            }
    }

}

export default configBuilder;
