module.exports = {
    parser: '@typescript-eslint/parser',
    extends: [
        'eslint:recommended',
        "plugin:react/recommended",
        'plugin:import/typescript',
        'plugin:@typescript-eslint/recommended',
        'prettier/@typescript-eslint',
        'plugin:prettier/recommended'
    ],
    plugins: [
        'react'
    ],
    env: {
        browser: true,
    },
    parserOptions: {
        ecmaVersion: 2018,
        sourceType: 'module',
        ecmaFeatures: {
            jsx: true
        },
    },
    rules: {
        'no-undef': 'off', // https://github.com/typescript-eslint/typescript-eslint/issues/342,,
        '@typescript-eslint/explicit-function-return-type': ['error', { allowTypedFunctionExpressions: true, allowHigherOrderFunctions: true }],
        '@typescript-eslint/prefer-interface': ['off'],
        '@typescript-eslint/no-use-before-define': ['off']
    },
    settings: {
        react: {
            pragma: 'h',
            version: 'latest',
        },
    }
};
