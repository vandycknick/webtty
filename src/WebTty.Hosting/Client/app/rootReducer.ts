import { combineReducers } from "redux"

import terminal from "features/terminal/terminalReducer"
import theme from "features/theme/themeReducer"

const rootReducer = combineReducers({ terminal, theme })
type RootState = ReturnType<typeof rootReducer>
type RootAction = Parameters<typeof rootReducer>[1]

export { RootState, RootAction }
export default rootReducer
