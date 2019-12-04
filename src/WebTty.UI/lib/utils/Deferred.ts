/* eslint-disable @typescript-eslint/no-explicit-any */
type Resolver<T> = (value?: T | PromiseLike<T> | undefined) => void
type Rejector<T> = (reason?: T) => void

class Deferred<T> implements Promise<T> {
    private _resolveSelf: Resolver<T> = Promise.resolve
    private _rejectSelf: Rejector<T> = Promise.reject
    private promise: Promise<T>

    public constructor() {
        this.promise = new Promise<T>((resolve, reject): void => {
            this._resolveSelf = resolve
            this._rejectSelf = reject
        })
    }

    public then<TResult1 = T, TResult2 = never>(
        onfulfilled?: ((value: T) => TResult1 | PromiseLike<TResult1>) | undefined | null,
        onrejected?: ((reason: any) => TResult2 | PromiseLike<TResult2>) | undefined | null,
    ): Promise<TResult1 | TResult2> {
        return this.promise.then(onfulfilled, onrejected)
    }

    public catch<TResult = never>(
        onrejected?: ((reason: any) => TResult | PromiseLike<TResult>) | undefined | null,
    ): Promise<T | TResult> {
        return this.promise.then(onrejected)
    }

    public finally(onfinally?: (() => void) | null | undefined): Promise<T> {
        return this.promise.finally(onfinally)
    }

    public resolve(val: T): void {
        this._resolveSelf(val)
    }

    public reject(reason: any): void {
        this._rejectSelf(reason)
    }

    public [Symbol.toStringTag]: "Promise"
}

export default Deferred
