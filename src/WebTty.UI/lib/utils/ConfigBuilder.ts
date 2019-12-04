type ConfigValidator<T> = (config: Partial<T>) => config is T

class ConfigBuilder<TConfig> {
    private configMap: Partial<TConfig> = {}

    addVariable(key: keyof TConfig, value: TConfig[typeof key]): this {
        this.configMap[key] = value
        return this
    }

    addVariableDevelopment(
        key: keyof TConfig,
        value: TConfig[typeof key],
    ): this {
        if (process.env.NODE_ENV === "development") {
            this.addVariable(key, value)
        }
        return this
    }

    addFromDom(id: string): this {
        const $el = document.getElementById(id)

        if ($el !== null) {
            const config: Partial<TConfig> = JSON.parse($el.innerText)
            Object.assign(this.configMap, config)
        }
        return this
    }

    build(validator: ConfigValidator<TConfig>): TConfig {
        const config = Object.assign({}, this.configMap)
        if (!validator(config)) {
            throw new Error("Config validation error")
        }

        return config
    }
}

export default ConfigBuilder
