import { h, ComponentChildren, FunctionComponent } from "preact"
import { useRef, useEffect } from "preact/hooks"
import SizeObserver from "resize-observer-polyfill"

type ResizeObserverProps = {
    onChange: (entries?: ResizeObserverEntry[]) => void
    children: ComponentChildren
}

const ResizeObserver: FunctionComponent<ResizeObserverProps> = ({
    onChange,
    children,
}: ResizeObserverProps) => {
    const wrapperRef = useRef<HTMLDivElement>()

    useEffect(() => {
        if (wrapperRef.current == undefined) return

        const observer = new SizeObserver((entries): void => onChange(entries))

        observer.observe(wrapperRef.current)

        return (): void => observer.disconnect()
    }, [onChange])

    return <div ref={wrapperRef}>{children}</div>
}

export default ResizeObserver
