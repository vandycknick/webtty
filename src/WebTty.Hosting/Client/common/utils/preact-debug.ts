if (process.env.NODE_ENV === "development") {
    // Must use require here as import statements are only allowed
    // to exist at the top of a file.
    require("preact/debug")
}
