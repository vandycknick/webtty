/* eslint-disable */
const path = require("path")
const fs = require("fs")
const { DefinePlugin } = require("webpack")
const HtmlWebpackPlugin = require("html-webpack-plugin")
const TsconfigPathsPlugin = require("tsconfig-paths-webpack-plugin")

const readTemplateContent = indexPath => {
    const buffer = fs.readFileSync(indexPath)
    const template = buffer.toString("utf-8")
    const templateSource = template.slice(template.indexOf("<!doctype html>"))
    return templateSource
}

module.exports = (_, argv) => {
    const mode = argv.mode === "production" ? "production" : "development";

    const config = {
        mode,

        devtool: mode === "production" ? "source-map" : "inline-source-map",

        entry: "./index.tsx",

        output: {
            path: path.join(__dirname, "wwwroot"),
            filename: "[name].[chunkhash].js",
        },

        resolve: {
            extensions: [".ts", ".tsx", ".js"],
            alias: {
                react: "preact/compat",
                "react-dom": "preact/compat",
            },
            plugins: [new TsconfigPathsPlugin()],
        },

        module: {
            rules: [
                {
                    test: /\.tsx?$/,
                    exclude: /node_modules/,
                    use: [{ loader: "ts-loader" }],
                },
                {
                    test: /\.css$/,
                    use: ["style-loader", "css-loader"],
                },
            ],
        },

        plugins: [
            new DefinePlugin({
                "process.env.NODE_ENV": JSON.stringify(
                    argv.mode === "production" ? "production" : "development",
                ),
            }),

            ...(mode === "development"
                ? [
                      new HtmlWebpackPlugin({
                          templateContent: readTemplateContent(
                              "Pages/Index.cshtml",
                          ),
                      }),
                  ]
                : []),
        ],

        devServer: {
            headers: {
                "Access-Control-Allow-Origin": "*",
            },
            port: 3000,
            proxy: {
                "/tty": "http://localhost:5000",
            },
        },
    }

    return config
}
