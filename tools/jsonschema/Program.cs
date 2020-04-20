using jsonschema.FluentValidation;
using McMaster.NETCore.Plugins;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.Generation;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace jsonschema
{
    class Program
    {
        static int Main(string[] args)
        {
            var command = new RootCommand("webtty-message-gen")
            {
                new Option(
                    "--assembly",
                    "Absolute path to assembly containing json schema messages"
                )
                {
                    Argument = new Argument<string>(),
                    Required = true,
                },
                new Option(
                    "--namespace",
                    "Namespace containing json schema messages"
                )
                {
                    Argument = new Argument<string>(),
                    Required = true,
                },
                new Option(
                    "--output",
                    "Output folder for generated ts messages"
                )
                {
                    Argument = new Argument<string>(),
                    Required = true,
                }
            };

            command.Handler = CommandHandler.Create<string , string, string>(Generate);

            return command.Invoke(args);
        }

        public static void Generate(string assembly, string @namespace, string output)
        {
            var dir = Directory.GetCurrentDirectory();

            var loader = PluginLoader.CreateFromAssemblyFile(assembly);

            var defaultAssembly = loader.LoadDefaultAssembly();
            var messages =
                from t in defaultAssembly.GetTypes()
                where t.IsClass || t.IsValueType
                where !t.IsPrimitive && !string.IsNullOrEmpty(t.Namespace)
                where t.Namespace.StartsWith(@namespace)
                let attributes = t.GetCustomAttributes(typeof(DataContractAttribute), true)
                where attributes != null && attributes.Length > 0
                select t;

            var validatorFactory = new ValidatorFactory(defaultAssembly);
            var schemaProcessor = new FluentValidationSchemaProcessor(validatorFactory);

            var settings = new JsonSchemaGeneratorSettings();
            settings.SchemaProcessors.Add(schemaProcessor);
            settings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;

            var schema = new JsonSchema(); // the schema to write into
            var resolver = new JsonSchemaResolver(schema, settings); // used to add and retrieve schemas from the 'definitions'
            var generator = new JsonSchemaGenerator(settings);
            var tsSettings = new TypeScriptGeneratorSettings
            {
                TypeStyle = TypeScriptTypeStyle.Class,
                TypeScriptVersion = 3.7m,
            };

            Directory.CreateDirectory(output);

            var mainModule = "";
            foreach (var type in messages)
            {
                var schemaForType = generator.Generate(type, resolver);
                schemaForType.Title = type.Name;

                var codeGenerator = new TypeScriptGenerator(schemaForType, tsSettings);
                var code = codeGenerator.GenerateFile();


                using (var sourceFile = File.Open($"{output}/{type.Name}.ts", FileMode.Create))
                {
                    sourceFile.Write(Encoding.UTF8.GetBytes(code));
                    sourceFile.Flush();
                }

                mainModule += $"export * from './{type.Name}'\n";
            }

            using (var mainFile = File.Open($"{output}/index.ts", FileMode.Create))
            {
                mainFile.Write(Encoding.UTF8.GetBytes(mainModule));
                mainFile.Flush();
            }

            Console.WriteLine(schema.ToJson());
        }
    }
}
