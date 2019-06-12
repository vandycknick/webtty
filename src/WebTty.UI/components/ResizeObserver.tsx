import { h, ComponentChildren } from "preact"
import { useRef, useEffect } from "preact/hooks"
import SizeObserver from 'resize-observer-polyfill'

console.log(SizeObserver);

type ResizeObserverProps = {
    onChange: (entries?: ResizeObserverEntry[]) => void
    children: ComponentChildren
}

const ResizeObserver = (props: ResizeObserverProps) => {
    const wrapperRef = useRef<HTMLDivElement>();

    useEffect(() => {
        if (wrapperRef.current == undefined) return

        const observer = new SizeObserver((entries) => props.onChange(entries))

        observer.observe(wrapperRef.current)

        return () => observer.disconnect()
    }, [wrapperRef.current])

    return <div ref={wrapperRef}>{props.children}</div>
}

export default ResizeObserver
