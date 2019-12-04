import { createContext } from "preact"
import { useContext } from "preact/hooks"

const Context = createContext<any>({})

const useServices = <T>(): T => {
    const services = useContext<T>(Context)
    return services
}

const ServiceProvider = Context.Provider

export { ServiceProvider }
export default useServices
