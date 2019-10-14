using Altered.Pipeline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Altered.Pipeline
{
    public static class ProcessExtensions
    {
        public static IAlteredPipeline<Memory<byte>, Memory<byte>> AsProcess(/*this IAlteredPipeline<Memory<byte>, Memory<byte>> pipeline,*/ string filename, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
              FileName = filename,
              Arguments = arguments,
              RedirectStandardInput = true,
              //RedirectStandardOutput = true // todo
              //RedirectStandardError = true, // todo
              //Environment = new Dictionary<string, string>
              //{
              //    {  nameof(AlteredEnvironment.Repo), AlteredEnvironment.Repo },
              //    { nameof(AlteredEnvironment.Env), AlteredEnvironment.Env },
              //    {nameof(AlteredEnvironment.Sha), AlteredEnvironment.Sha }
              //}
              //UseShellExecute = 
            };
            var process = Process.Start(startInfo);
            return new AlteredPipeline<Memory<byte>, Memory<byte>>(async (input) =>
            {
                // -> process.stdin; process.stdout -> outputs
                await process.StandardInput.BaseStream.WriteAsync(input.ToArray(), 0, input.Length);
                // todo read
                return input;
            });
        }
    }
}
