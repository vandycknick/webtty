import Deferred from "./Deferred"
import { IDisposable } from "common/types"

const isCancelledInternal = Symbol.for("isCancelled")

export class CancellationToken {
    private [isCancelledInternal] = false
    public readonly promise: Promise<void>

    constructor(promise: Promise<void>) {
        this.promise = promise
    }

    public get isCancelled(): boolean {
        return this[isCancelledInternal]
    }
}

export class CancellationTokenSource implements IDisposable {
    private readonly _deferred: Deferred<void>
    private readonly _token: CancellationToken

    constructor() {
        this._deferred = new Deferred()
        this._token = new CancellationToken(this._deferred)

        this._deferred.then(() => (this._token[isCancelledInternal] = true))
    }

    public get isCancellationRequested(): boolean {
        return this._token.isCancelled
    }

    public get token(): CancellationToken {
        return this._token
    }

    public cancel = (): void => this._deferred.resolve()
    public promise = (): Promise<void> => this._deferred
    public dispose(): void {
        if (!this._token.isCancelled) {
            this.cancel()
        }
    }
}
