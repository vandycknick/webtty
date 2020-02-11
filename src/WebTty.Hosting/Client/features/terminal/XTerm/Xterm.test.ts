import detectWebgl2Support from "common/utils/detectWebgl2Support"
import XTerm from "./XTerm"
import * as xterm from "xterm"
import * as addon from "xterm-addon-fit"
import { WebglAddon } from "xterm-addon-webgl"

jest.mock("xterm")
jest.mock("xterm-addon-fit")
jest.mock("common/utils/detectWebgl2Support")

describe("XTerm", () => {
    let detectWebgl2SupportMock: jest.Mock
    let TerminalMock: jest.Mock<xterm.Terminal>

    beforeEach(() => {
        detectWebgl2SupportMock = detectWebgl2Support as jest.Mock
        detectWebgl2SupportMock.mockReturnValue(false)
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        TerminalMock = (xterm.Terminal as any) as jest.Mock<xterm.Terminal>
    })

    afterEach(() => {
        jest.useRealTimers()
        jest.restoreAllMocks()
        jest.resetAllMocks()
    })

    it("opens and attaches the terminal to the given element when not previously opened", () => {
        // Given
        const parent = {
            appendChild: jest.fn(),
        }
        const term = new XTerm()

        // When
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        term.open(parent as any)

        // Then
        const instance = TerminalMock.mock.instances[0]
        expect(parent.appendChild).toHaveBeenCalledTimes(1)
        expect(parent.appendChild).toHaveBeenCalledWith(
            expect.any(HTMLDivElement),
        )
        expect(instance.open).toHaveBeenCalledTimes(1)
        expect(instance.loadAddon).toHaveBeenCalledTimes(1)
        expect(instance.loadAddon).toHaveBeenCalledWith(
            expect.any(addon.FitAddon),
        )
    })

    it("only attaches to the given parent element when the terminal is already opened", () => {
        // Given
        const parent = {
            appendChild: jest.fn(),
        }
        const term = new XTerm()

        // When
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        term.open(parent as any)
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        term.open(parent as any)

        // Then
        const instance = TerminalMock.mock.instances[0]
        expect(parent.appendChild).toHaveBeenNthCalledWith(
            1,
            expect.any(HTMLDivElement),
        )
        expect(parent.appendChild).toHaveBeenNthCalledWith(
            2,
            expect.any(HTMLDivElement),
        )
        expect(instance.open).toHaveBeenCalledTimes(1)
    })

    it("uses the WebglAddon if webgl2 is supported", () => {
        // Given
        const parent = {
            appendChild: jest.fn(),
        }
        detectWebgl2SupportMock.mockReturnValue(true)
        const term = new XTerm()

        // When
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        term.open(parent as any)

        // Then
        const instance = TerminalMock.mock.instances[0]
        expect(instance.loadAddon).toHaveBeenNthCalledWith(
            2,
            expect.any(WebglAddon),
        )
    })

    it("resizes to fit when the resize method is called", () => {
        // Given
        const term = new XTerm()

        // When
        term.open(document.createElement("div"))
        term.resize()

        // Then
        const instance = (addon.FitAddon as jest.Mock<addon.FitAddon>).mock
            .instances[0]
        expect(instance.fit).toHaveBeenCalled()
    })

    it("does nothing when resize is called but terminal is not attached to the dom", () => {
        // Given
        const term = new XTerm()

        // When, Then
        expect(() => term.resize()).not.toThrow()
    })

    it("focuses the terminal", () => {
        // Given
        const term = new XTerm()

        // When
        term.focus()

        // Then
        const instance = TerminalMock.mock.instances[0]
        expect(instance.focus).toHaveBeenCalled()
    })

    it("disposes the terminal", () => {
        // Given
        const term = new XTerm()

        // When
        term.dispose()

        // Then
        const instance = TerminalMock.mock.instances[0]
        expect(instance.dispose).toHaveBeenCalled()
    })

    it("registers the given theme", () => {
        // Given
        const term = new XTerm()
        const theme = {
            background: "black",
        }

        // When
        term.setTheme(theme)

        // Then
        const instance = TerminalMock.mock.instances[0]
        expect(instance.setOption).toHaveBeenCalledWith("theme", theme)
    })

    it("buffers write calls before flushing to the terminal", () => {
        // Given
        jest.useFakeTimers()
        const term = new XTerm()

        // When
        term.write("h")
        term.write("e")
        term.write("l")
        jest.runOnlyPendingTimers()

        term.write("l")
        term.write("o")
        jest.runOnlyPendingTimers()

        // Then
        const instance = TerminalMock.mock.instances[0]
        expect(instance.write).toHaveBeenCalledTimes(2)
        expect(instance.write).toHaveBeenNthCalledWith(1, "hel")
        expect(instance.write).toHaveBeenNthCalledWith(2, "lo")
    })

    it("registers a data callback", () => {
        // Given
        const termMock = { onData: jest.fn() }
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        TerminalMock.mockImplementation(() => termMock as any)
        const term = new XTerm()

        // When
        term.onData(jest.fn())

        // Then
        expect(termMock.onData).toHaveBeenCalled()
    })

    it("registers a resize callback", () => {
        // Given
        const termMock = { onResize: jest.fn() }
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        TerminalMock.mockImplementation(() => termMock as any)
        const term = new XTerm()

        // When
        term.onResize(jest.fn())

        // Then
        expect(termMock.onResize).toHaveBeenCalled()
    })

    it("registers a title changed callback", () => {
        // Given
        const termMock = { onTitleChange: jest.fn() }
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        TerminalMock.mockImplementation(() => termMock as any)
        const term = new XTerm()

        // When
        term.onTitleChange(jest.fn())

        // Then
        expect(termMock.onTitleChange).toHaveBeenCalled()
    })

    it("detaches itself from the dom", () => {
        // Given
        const parent = document.createElement("div")
        const term = new XTerm()

        // When
        term.open(parent)
        expect(parent.childNodes).toHaveLength(1)

        term.detach()

        // Then
        expect(parent.childNodes).toHaveLength(0)
    })

    it("cleans up any registered callbacks on detach", () => {
        // Given
        const disposable = { dispose: jest.fn() }
        const termMock = { onData: jest.fn().mockReturnValue(disposable) }
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        TerminalMock.mockImplementation(() => termMock as any)
        const term = new XTerm()

        // When
        term.onData(jest.fn())
        term.detach()

        // Then
        expect(disposable.dispose).toHaveBeenCalled()
    })
})
