/* eslint-disable */
const { pathsToModuleNameMapper } = require("ts-jest/utils")
const { compilerOptions } = require("./tsconfig.json")

module.exports = {
    preset: "ts-jest",
    globals: {
        "ts-jest": {
            tsConfig: "tsconfig.json",
        },
    },
    collectCoverageFrom: ["**/*.{tsx,ts}"],
    setupFiles: ["<rootDir>/setupTests.ts"],
    moduleNameMapper: {
        ...pathsToModuleNameMapper(compilerOptions.paths, {
            prefix: "<rootDir>",
        }),
        "^.+\\.css$": "identity-obj-proxy",
        "^.+\\.scss$": "identity-obj-proxy",
        "^.+\\.svg$": "identity-obj-proxy",
    },
    testPathIgnorePatterns: ["node_modules", "__mocks__"],
}
