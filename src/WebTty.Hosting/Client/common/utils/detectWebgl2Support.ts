const detectWebgl2Support = (): boolean => {
    const canvas = document.createElement("canvas")
    const gl = canvas.getContext("webgl2")
    return !!gl
}

export default detectWebgl2Support
