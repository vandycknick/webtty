import { useState, useEffect, useMemo } from "preact/hooks/src"

export const useDebouncedValue = <T>(value: T, delay: number): T => {
    const [debouncedValue, setDebouncedValue] = useState(value)

    useEffect(() => {
        const handler = setTimeout(() => setDebouncedValue(value), delay)

        return (): void => clearTimeout(handler)
    }, [value, delay]) // Only re-call effect if value or delay changes

    return debouncedValue
}

export const useDebounce = <T extends Function>(handle: T, delay: number): T => {
    const wrappedHandle = useMemo(() => {
        let timeout: number
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        const wrapped = (...args: any[]): void => {
            window.clearTimeout(timeout)
            timeout = window.setTimeout(() => {
                timeout = -1
                handle(...args)
            }, delay)
        }

        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        return (wrapped as any) as T
    }, [handle, delay])

    return wrappedHandle
}
