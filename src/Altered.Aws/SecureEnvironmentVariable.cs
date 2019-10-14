using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Nito.AsyncEx;

namespace Altered.Aws
{
    public sealed class SecureEnvironmentVariable
    {
        readonly AsyncLazy<string> lazyValue;

        public SecureEnvironmentVariable(IAmazonSimpleSystemsManagement ssm, string name)
        {
            lazyValue = new AsyncLazy<string>(async () =>
            {
                var request = new GetParameterRequest
                {
                    Name = name,
                    WithDecryption = true
                };
                var response = await ssm.GetParameterAsync(request);
                return response.Parameter?.Value;
            });
        }

        public Task<string> GetValue() => lazyValue.Task;
    }
}
