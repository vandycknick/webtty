using System.Threading.Tasks;
using BtrExec;

namespace WebTty.Api.Common
{
    public static class IProcessExtensions
    {
        public static async ValueTask WaitUntilReady(this IProcess proc)
        {
            while (!proc.IsRunning)
            {
                await Task.Delay(1);
            }
        }
    }
}
