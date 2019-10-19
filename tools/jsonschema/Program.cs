using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.Generation;
using WebTty;
using WebTty.Messages;

namespace jsonschema
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = "src/WebTty.UI/.tmp/messages";
            var settings = new JsonSchemaGeneratorSettings();

            var schema = new JsonSchema(); // the schema to write into
            var resolver = new JsonSchemaResolver(schema, settings); // used to add and retrieve schemas from the 'definitions'
            var generator = new JsonSchemaGenerator(settings);
            var tsSettings = new TypeScriptGeneratorSettings
            {
                TypeStyle = TypeScriptTypeStyle.Class,
                TypeScriptVersion = 3.5m,
            };

            var mainModule = "";

            foreach (var type in GetMessages<Startup>())
            {
                generator.Generate(type, resolver);
                var codeGenerator = new TypeScriptGenerator(JsonSchema.FromType(type), tsSettings);
                var code = codeGenerator.GenerateFile();

                Directory.CreateDirectory(root);

                using (var sourceFile = File.Open($"{root}/{type.Name}.ts", FileMode.Create))
                {
                    sourceFile.Write(Encoding.UTF8.GetBytes(code));
                    sourceFile.Flush();
                }

                mainModule += $"export * from './{type.Name}'\n";
            }

            using (var mainFile = File.Open($"{root}/index.ts", FileMode.Create))
            {
                mainFile.Write(Encoding.UTF8.GetBytes(mainModule));
                mainFile.Flush();
            }

            Console.WriteLine(schema.ToJson());
        }

        static List<Type> GetMessages<TFromAssembly>()
        {
            var assembly = Assembly.GetAssembly(typeof(TFromAssembly));

            var types =
                from t in assembly.GetTypes()
                where t.IsClass && !string.IsNullOrEmpty(t.Namespace)
                let attributes = t.GetCustomAttributes(typeof(MessageAttribute), true)
                where attributes != null && attributes.Length > 0
                select t;

            return types.ToList();
        }
    }
}
