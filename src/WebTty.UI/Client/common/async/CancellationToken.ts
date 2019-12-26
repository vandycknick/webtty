import Deferred from "./Deferred"
import { IDisposable } from "common/types"

const isCancelledInternal = Symbol.for("isCancelled")
const tokenPromiseInternal = Symbol.for("tokenPromise")

export class CancellationToken {
    private [isCancelledInternal] = false
    private [tokenPromiseInternal]: Promise<void>

    public promise = (): Promise<void> => this[tokenPromiseInternal]

    public get isCancelled(): boolean {
        return this[isCancelledInternal]
    }
}

export class CancellationTokenSource implements IDisposable {
    private readonly _deferred: Deferred<void>
    private readonly _token: CancellationToken

    constructor() {
        this._deferred = new Deferred()
        this._token = new CancellationToken()
        this._token[isCancelledInternal] = false
        this._token[tokenPromiseInternal] = this._deferred

        this._deferred.then(() => (this._token[isCancelledInternal] = true))
    }

    public get isCancellationRequested(): boolean {
        return this._token.isCancelled
    }

    public get token(): CancellationToken {
        return this._token
    }

    public cancel = (): void => this._deferred.resolve()
    public dispose = (): void => this.cancel()
}
