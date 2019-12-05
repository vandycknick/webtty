import { createContext } from "preact"
import { useContext } from "preact/hooks"

const Context = createContext<unknown | undefined>(undefined)

const useServices = <T>(): T => {
    const services = useContext(Context)

    if (services === undefined) {
        throw new Error("No services provided")
    }
    return services as T
}

const ServiceProvider = Context.Provider

export { ServiceProvider }
export default useServices
