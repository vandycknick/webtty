import "jest-canvas-mock"
import { TextEncoder, TextDecoder } from "util"

window["TextEncoder"] = TextEncoder
// eslint-disable-next-line @typescript-eslint/no-explicit-any
window["TextDecoder"] = TextDecoder as any
