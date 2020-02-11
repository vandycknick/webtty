import { createStore, AnyAction } from "redux"
import rootReducer from "./rootReducer"
import terminal from "features/terminal/terminalReducer"
import theme from "features/theme/themeReducer"

describe("rootReducer", () => {
    it("should have the correct initial state (smoke test)", () => {
        // Given
        const dummyAction: AnyAction = {
            type: "dummy_action",
        }

        // When
        const store = createStore(rootReducer)

        // Then
        expect(store.getState()).toEqual({
            terminal: terminal(undefined, dummyAction),
            theme: theme(undefined, dummyAction),
        })
    })
})
