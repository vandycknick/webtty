# 3. Coding Style and Enforcement

Date: 21/10/2019

## Status

Proposed

## Context

This codebase should be easily approachable by all those contributing to it current and future, consistency of coding style is thus an important aspect.

## Decision
For JavaScript/TypeScript based projects we will use eslint to enforce consistent coding styles and prettier to enforce consistent code formatting.

Code formatting with the following settings:
- Use spaces with a tab width of 4
- A maximum line length of 120
- Enforce double quotes with trailing commas, without semi colons

Code style with the following recommended settings:
- eslint:recommended
- react/recommended
- import/typescript
- @typescript-eslint/recommended
- prettier/@typescript-eslint
- prettier/recommended

With the following overrides:
- allow explicit function return type for expressions, typed function expressions and higher order functions
- Do not prefer interfaces
- allow function usage before definition

For C# based projects ... (TODO)

Commits introducing code that does not adhere to the style guide will fail the build.

## Consequences

Developers will need to ensure their commits adhere to the style guide, this can be achieved by running `nuke lint` or `nuke check`.

It's important to continuously assess whether individual rules are working for this project and introduce further modifications or exemptions where agreed. Any further modifications will be documented in future ADRs.
