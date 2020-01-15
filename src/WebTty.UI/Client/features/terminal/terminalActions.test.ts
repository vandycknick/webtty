import {
    setStatus,
    TERMINAL_SET_STATUS,
    openNewTab,
    TERMINAL_SEND_MESSAGE,
    TERMINAL_TAB_CREATED,
    newTab,
    resizeTab,
    writeStdIn,
} from "./terminalActions"
import {
    OpenNewTabRequest,
    ResizeTabRequest,
    SendInputRequest,
} from "@webtty/messages"

describe("setStatus", () => {
    it("creates a TERMINAL_STATUS action", () => {
        // Given
        const status = "connecting"

        // When
        const action = setStatus(status)

        // Then
        expect(action).toEqual({
            type: TERMINAL_SET_STATUS,
            payload: status,
        })
    })
})

describe("openNewTab", () => {
    it("creates a TERMINAL_SEND_MESSAGE action with an OpenNewTabRequest", () => {
        // Given, When
        const action = openNewTab()

        // Then
        expect(action).toEqual({
            type: TERMINAL_SEND_MESSAGE,
            payload: new OpenNewTabRequest(),
        })
    })
})

describe("newTab", () => {
    it("creates a TERMINAL_TAB_CREATED action", () => {
        // Given, When
        const action = newTab("123")

        // Then
        expect(action).toEqual({
            type: TERMINAL_TAB_CREATED,
            payload: {
                id: "123",
            },
        })
    })
})

describe("resizeTab", () => {
    it("creates a TERMINAL_SEND_MESSAGE action with a ResizeTabMessage", () => {
        // Given
        const tabId = "123"
        const cols = 23
        const rows = 40

        // When
        const action = resizeTab(tabId, cols, rows)

        // Then
        expect(action).toEqual({
            type: TERMINAL_SEND_MESSAGE,
            payload: new ResizeTabRequest({ tabId, cols, rows }),
        })
    })
})

describe("writeStdIn", () => {
    it("creates a TERMINAL_SEND_MESSAGE action with a StdInputRequest", () => {
        // Given
        const tabId = "123"
        const payload = "hello"

        // When
        const action = writeStdIn(tabId, payload)

        // Then
        expect(action).toEqual({
            type: TERMINAL_SEND_MESSAGE,
            payload: new SendInputRequest({ tabId, payload }),
        })
    })
})
