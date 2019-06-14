interface IEventEmitter {
    addEventListener(event: string, listener: (...args: any[]) => void): void;
    removeEventListener(event: string, listener: (...args: any[]) => void): void;
}

const $terminated = Symbol.for("terminated");

const oncePromise = <T>(emitter: IEventEmitter, event: string): Promise<T> => {
    return new Promise(resolve => {
        var handler = (...args: any[]) => {
            emitter.removeEventListener(event, handler);
            resolve(...args);
        };
        emitter.addEventListener(event, handler);
    });
};

function fromEmitter<T>(eventEmitter: IEventEmitter): AsyncIterable<T | typeof $terminated> {
    let disposed = false;
    const buffer: (T | typeof $terminated)[] = [];
    let error: any;

    const nextListener = (...args: T[]) => buffer.push(...args);
    const errorListener = (...args: any) => {
        if (disposed) return;
        error = args[0];
        dispose();
    }
    const doneListener = () => {
        if (disposed) return;
        buffer.push($terminated)
        dispose();
    }
    const dispose = () => {
        eventEmitter.removeEventListener("message", nextListener);
        eventEmitter.removeEventListener("eror", errorListener);
        eventEmitter.removeEventListener("close", doneListener);
        disposed = true
    }

    eventEmitter.addEventListener("message", nextListener);
    eventEmitter.addEventListener("error", errorListener);
    eventEmitter.addEventListener("close", doneListener);

    return {
        [Symbol.asyncIterator]: async function* iterator() {
            while (!disposed) {
                if (error) {
                    dispose();
                    throw error;
                }
                if (buffer.length === 0) {
                    if (disposed) {
                        dispose();
                        return;
                    };
                    await oncePromise(eventEmitter, "message");
                } else {
                    const data = yield buffer.shift();
                    if (data === $terminated) {
                        dispose();
                        return;
                    }

                    if (data !== undefined) {
                        yield data
                    }
                }
            }
        }
    }
}

export { $terminated }
export default fromEmitter
