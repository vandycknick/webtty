# 3. Coding Style and Enforcement

Date: 21/10/2019

## Status

Proposed

## Context

This codebase should be easily approachable by all those contributing to it current and future, consistency of coding style is thus an important aspect.

## Decision
For TypeScript based projects we will use eslint to enforce consistent coding styles and prettier to enforce consistent code formatting.

For C# based projects I have not made a decision yet, this is something that should be addressed in a future extension.

An editorconfig file will be available where needed so that editors can pick up common settings.

These rules should not be written in stone. In the future, it should be possible given consensus to add/remove or override certain rules.

Commits introducing code that does not adhere to the above settings should fail the CI build.

## Consequences
Not having linting an coding style automated setup for C# can haunt me in the future, because of all the styling dept that will be accrued. But I simply don't have the bandwidth or knowledge a the moment to get this setup.
