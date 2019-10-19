using Microsoft.AspNetCore.Http;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace WebTty.Transport
{
    public interface ITransport
    {
          Task ProcessAsync(HttpContext context);
    }
}
