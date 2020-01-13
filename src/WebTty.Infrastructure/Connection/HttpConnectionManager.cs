using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace WebTty.Infrastructure.Connection
{
    internal class HttpConnectionManager
    {
        private readonly ConcurrentDictionary<string, IConnectionContext> _connections = new ConcurrentDictionary<string, IConnectionContext>();
        private readonly IServiceProvider _provider;
        public HttpConnectionManager(IServiceProvider provider)
        {
            _provider = provider;
        }

        public bool TryGet(string id, out IConnectionContext context)
        {
            return _connections.TryGetValue(id, out context);
        }

        internal IConnectionContext GetOrCreate(string id)
        {
            if (!TryGet(id, out IConnectionContext context))
            {
                context = _provider.GetRequiredService<IConnectionContext>();
                _connections.TryAdd(id, context);
            }

            return context;
        }

        internal void Remove(string id)
        {
            if (_connections.TryRemove(id, out var context))
            {
                context.Close();
            }
        }
    }
}
