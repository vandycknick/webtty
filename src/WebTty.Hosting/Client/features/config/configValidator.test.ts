import { configValidator } from "./configValidator"
import { AppConfig } from "./AppConfig"

describe("configValidator", () => {
    it("throws an error when ptyHost is undefined", () => {
        // Given
        const config: Partial<AppConfig> = {}

        // When, Then
        expect(() => configValidator(config)).toThrow(
            "AppConfig ptyHost is not defined",
        )
    })

    it("throws an error when ptyPath is undefined", () => {
        // Given
        const config: Partial<AppConfig> = {
            ptyHost: "something",
        }

        // When, Then
        expect(() => configValidator(config)).toThrow(
            "AppConfig ptyPath is not defined",
        )
    })

    it("throws an error when theme is undefined", () => {
        // Given
        const config: Partial<AppConfig> = {
            ptyHost: "something",
            ptyPath: "path",
        }

        // When, Then
        expect(() => configValidator(config)).toThrow(
            "AppConfig selectedTheme is not defined",
        )
    })

    it("returns true when the config is valid", () => {
        // Given
        const config: Partial<AppConfig> = {
            ptyHost: "something",
            ptyPath: "path",
            selectedTheme: "default",
        }

        // When
        const result = configValidator(config)

        // Then
        expect(result).toBe(true)
    })
})
