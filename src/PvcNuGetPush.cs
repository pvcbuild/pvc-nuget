using PvcCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edokan.KaiZen.Colors;

namespace PvcPlugins
{
    public class PvcNuGetPush : PvcPlugin
    {
        private readonly string serverUrl;
        private readonly string apiKey;
        private readonly string symbolServerUrl;
        private readonly string symbolApiKey;
        private readonly TimeSpan? timeout;

        public PvcNuGetPush(
            string serverUrl = null,
            string apiKey = null,
            string symbolServerUrl = null,
            string symbolApiKey = null,
            TimeSpan? timeout = null
            )
        {
            this.serverUrl = serverUrl ?? PvcNuGet.ServerUrl;
            this.apiKey = apiKey ?? PvcNuGet.ApiKey;
            this.symbolServerUrl = symbolServerUrl ?? PvcNuGet.SymbolServerUrl;
            this.symbolApiKey = (symbolApiKey ?? PvcNuGet.SymbolApiKey) ?? this.apiKey;
            this.timeout = timeout.HasValue ? timeout : PvcNuGet.Timeout;
        }

        public override IEnumerable<PvcStream> Execute(IEnumerable<PvcStream> inputStreams)
        {
            var packageStreams = inputStreams.Where(x => x.StreamName.EndsWith(".nupkg"));

            foreach (var packageStream in packageStreams)
            {
                var isSymbolPackage = packageStream.StreamName.EndsWith(".symbols.nupkg");
                var args = new List<string>(new[] {
                    "push",
                    packageStream.StreamName,
                    isSymbolPackage ? this.symbolApiKey : this.apiKey
                });

                if (!isSymbolPackage && this.serverUrl != null) args.AddRange(new[] { "-Source", this.serverUrl });
                if (isSymbolPackage && this.symbolServerUrl != null) args.AddRange(new[] { "-Source", this.symbolServerUrl });

                if (this.timeout.HasValue) args.AddRange(new[] { "-Timeout", this.timeout.Value.TotalSeconds.ToString() });
                args.Add("-NonInteractive");

                var resultStreams = PvcUtil.StreamProcessExecution(PvcNuGet.NuGetExePath, Directory.GetCurrentDirectory(), args.ToArray());

                string outLine;
                var outStreamReader = new StreamReader(resultStreams.Item1);
                while ((outLine = outStreamReader.ReadLine()) != null)
                {
                    Console.WriteLine(outLine);
                }

                var errOutput = new StreamReader(resultStreams.Item2).ReadToEnd();
                if (errOutput.Trim().Length > 0)
                    throw new PvcException("NuGet Push Error: " + errOutput);
            }

            return inputStreams.Except(packageStreams);
        }
    }
}
