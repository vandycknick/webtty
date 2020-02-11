using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace WebTty.Api.Infrastructure.Connection
{
    internal class HttpConnectionManager
    {
        private readonly ConcurrentDictionary<string, ConnectionContext> _connections = new ConcurrentDictionary<string, ConnectionContext>();
        private readonly IServiceProvider _provider;
        public HttpConnectionManager(IServiceProvider provider)
        {
            _provider = provider;
        }

        public bool TryGet(string id, out ConnectionContext context)
        {
            return _connections.TryGetValue(id, out context);
        }

        internal ConnectionContext GetOrCreate(string id)
        {
            if (!TryGet(id, out var context))
            {
                context = _provider.GetRequiredService<ConnectionContext>();
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
