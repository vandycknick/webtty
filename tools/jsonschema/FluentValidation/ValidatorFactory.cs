using System;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace jsonschema.FluentValidation
{
    public class ValidatorFactory : ValidatorFactoryBase
    {
        private readonly ServiceProvider _provider;
        public ValidatorFactory(Assembly assembly)
        {
            var services = new ServiceCollection();
            AssemblyScanner.FindValidatorsInAssembly(assembly)
                .ForEach(result =>
                {
                    services.AddSingleton(result.InterfaceType, result.ValidatorType);
                });

            _provider = services.BuildServiceProvider();
        }

        public override IValidator CreateInstance(Type validatorType)
            => _provider.GetService(validatorType) as IValidator;
    }
}
