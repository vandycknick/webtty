interface IStream<T> {
    write(data: T): void
    [Symbol.asyncIterator](): AsyncIterableIterator<T>
}

export { IStream }
