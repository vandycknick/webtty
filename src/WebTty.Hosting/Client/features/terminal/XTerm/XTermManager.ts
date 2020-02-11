import { ITerminalManager, ITerminal } from "../types"
import XTerm from "./XTerm"

class XTermManager implements ITerminalManager {
    private readonly _store = new Map<string, ITerminal>()

    public get(id: string): ITerminal {
        let term = this._store.get(id)
        if (term === undefined) {
            term = new XTerm()
            this._store.set(id, term)
        }

        return term
    }

    public write(id: string, data: string): void {
        const term = this._store.get(id)
        if (term !== undefined) {
            term.write(data)
        }
    }
}

export default XTermManager
