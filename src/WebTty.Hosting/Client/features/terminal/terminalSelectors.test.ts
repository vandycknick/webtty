import { merge, cloneDeep } from "lodash/fp"

import { DeepPartial } from "common/types"
import { TerminalState, initialState } from "./terminalReducer"
import { getSelectedTab } from "./terminalSelectors"

describe("getSelectedTab", () => {
    const createState = (
        state: DeepPartial<TerminalState> = {},
    ): TerminalState => {
        return merge(
            cloneDeep({
                terminal: initialState,
            }),
            state,
        )
    }

    it("returns the currently selected tab", () => {
        // Given
        const state = createState({
            terminal: {
                selectedTab: "123",
            },
        })

        // When
        const tab = getSelectedTab(state)

        // Then
        expect(tab).toBe("123")
    })

    it("returns an empty string when no tab is selected", () => {
        // Given
        const state = createState()

        // When
        const tab = getSelectedTab(state)

        // Then
        expect(tab).toBe("")
    })
})
