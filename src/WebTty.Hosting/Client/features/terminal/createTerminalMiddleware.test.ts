import { OpenNewTabReply, OutputEvent } from "@webtty/messages"
import { Middleware } from "redux"

import { IConnection } from "common/connection"
import AsyncQueue from "common/async/AsyncQueue"
import { Messages } from "features/terminal/protocol"
import createTerminalMiddleware, {
    mapMessagesToActions,
} from "./createTerminalMiddleware"
import { newTab, openStdout, setStatus, openNewTab } from "./terminalActions"

import * as xterm from "features/terminal/XTerm"
import * as utils from "common/async/utils"
import { CancellationToken } from "common/async/CancellationToken"

jest.mock("features/terminal/XTerm")
jest.mock("common/async/utils")

describe("mapMessagesToActions", () => {
    afterEach(() => jest.resetAllMocks())

    it("returns the correct actions for a OpenNewTabReply message", async () => {
        // Given
        const messageStream = new AsyncQueue<Messages>()
        messageStream.enqueue(
            new OpenNewTabReply({ id: "123", parentId: "456" }),
        )
        messageStream.dispose()

        // When
        const actionStream = mapMessagesToActions(messageStream)

        // Then
        const actions = [newTab("123"), openStdout("123")]
        let cnt = 0
        for await (const action of actionStream) {
            expect(action).toEqual(actions[cnt])
            cnt++
        }
        expect(cnt).toBe(2)
    })

    it("writes any OutputEvent directly to the terminal and does not return an action", async () => {
        // Given
        const messageStream = new AsyncQueue<Messages>()
        messageStream.enqueue(
            new OutputEvent({ tabId: "123", data: [104, 101, 108, 108, 111] }),
        )
        messageStream.dispose()

        // When
        const actionStream = mapMessagesToActions(messageStream)

        // Then
        const message = await actionStream.next()
        expect(message).toEqual({
            done: true,
        })
        expect(xterm.termManager.write).toHaveBeenCalledWith("123", "hello")
    })
})

describe("createTerminalMiddleware", () => {
    let termMiddleware: Middleware
    let connection: IConnection
    let queue: AsyncQueue<string | ArrayBuffer>

    beforeEach(() => {
        queue = new AsyncQueue<string | ArrayBuffer>()
        connection = {
            state: 1,
            start: jest.fn().mockReturnValue(Promise.resolve()),
            send: jest.fn(),
            dispose: jest.fn(),
            next: queue.next,
            [Symbol.asyncIterator]: queue[Symbol.asyncIterator],
        }

        termMiddleware = createTerminalMiddleware({ connection })
    })

    afterAll(() => jest.resetAllMocks())

    it("starts a new connection on initialization", () => {
        // Given
        const dispatch = jest.fn()
        const getState = jest.fn()

        // When
        termMiddleware({ dispatch, getState })

        // Then
        expect(connection.start).toHaveBeenCalled()
    })

    it("dispatches a TERMINAL_SET_STATUS action when the connection is established", async () => {
        // Given
        const dispatch = jest.fn()
        const getState = jest.fn()

        // When
        termMiddleware({ dispatch, getState })
        await Promise.resolve() // await until the stubbed Promise from connection.start is resolved

        // Then
        expect(dispatch).toHaveBeenCalledWith(setStatus("connected"))
    })

    it("dispatches a TERMINAL_SEND_MESSAGE action with an OpenNewTabRequest when the connection is established", async () => {
        // Given
        const dispatch = jest.fn()
        const getState = jest.fn()

        // When
        termMiddleware({ dispatch, getState })
        await Promise.resolve() // await until the stubbed Promise from connection.start is resolved

        // Then
        const message = openNewTab(expect.any(String))
        expect(dispatch).toHaveBeenNthCalledWith(2, message)
    })

    it("starts consuming incoming messages", async () => {
        // Given
        const dispatch = jest.fn()
        const getState = jest.fn()

        // When
        termMiddleware({ dispatch, getState })
        await Promise.resolve() // await until the stubbed Promise from connection.start is resolved

        // Then
        expect(utils.consume).toHaveBeenCalledWith(
            expect.anything(),
            dispatch,
            expect.any(CancellationToken),
        )
    })

    it("returns a function to handle next", () => {
        // Given
        const dispatch = jest.fn()
        const getState = jest.fn()

        // When
        const nextHandler = termMiddleware({ dispatch, getState })

        // Then
        expect(nextHandler).toEqual(expect.any(Function))
        expect(nextHandler.length).toBe(1)
    })

    it("returns a function to handle actions", () => {
        // Given
        const dispatch = jest.fn()
        const getState = jest.fn()
        const next = jest.fn()

        // When
        const nextHandler = termMiddleware({ dispatch, getState })
        const actionHandler = nextHandler(next)

        // Then
        expect(actionHandler).toEqual(expect.any(Function))
        expect(nextHandler.length).toBe(1)
    })

    it("writes any TERMINAL_SEND_MESSAGE action into the connection", () => {
        // Given
        const dispatch = jest.fn()
        const getState = jest.fn()
        const next = jest.fn()

        // When
        const nextHandler = termMiddleware({ dispatch, getState })
        const actionHandler = nextHandler(next)

        actionHandler(openNewTab())
        actionHandler(openStdout("123"))

        // Then
        expect(connection.send).toHaveBeenCalledTimes(2)
    })

    it("calls next for any action that is not TERMINAL_SEND_MESSAGE", () => {
        // Given
        const dispatch = jest.fn()
        const getState = jest.fn()
        const next = jest.fn()

        // When
        const nextHandler = termMiddleware({ dispatch, getState })
        const actionHandler = nextHandler(next)

        actionHandler({ type: "test" })
        actionHandler({ type: "whatup" })

        // Then
        expect(next).toHaveBeenNthCalledWith(1, { type: "test" })
        expect(next).toHaveBeenNthCalledWith(2, { type: "whatup" })
    })
})
