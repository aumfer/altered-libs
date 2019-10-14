using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Shared.Helpers
{
    public static class ExceptionHelper
    {
        public static Exception DontThrow(Action doDangerousStuff)
        {
            try
            {
                doDangerousStuff();
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }

        public static async Task<Exception> DontThrow(Task doDangerousStuff)
        {
            try
            {
                await doDangerousStuff;
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }

        public static async Task<Exception> DontThrow(Func<Task> doDangerousStuff)
        {
            try
            {
                await doDangerousStuff();
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }

        public static async Task<(T, Exception)> DontThrow<T>(Func<Task<T>> doDangerousStuff)
        {
            try
            {
                var result = await doDangerousStuff();
                return (result, null);
            }
            catch (Exception e)
            {
                return (default(T), e);
            }
        }
    }
}
