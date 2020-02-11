import * as connection from "./index"
import ConnectionBuilder from "./ConnectionBuilder"

describe("connection", () => {
    it("exports a connectionbuilder", () => {
        expect(connection).toEqual({
            ConnectionBuilder,
        })
    })
})
