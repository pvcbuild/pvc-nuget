using PvcCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvcPlugins
{
    public static class PvcNuGet
    {
        private static string apiKey = null;
        public static string ApiKey
        {
            get
            {
                if (apiKey != null)
                    return apiKey;

                return Environment.GetEnvironmentVariable("NugetApiKey");
            }

            set { apiKey = value; }
        }

        public static string ServerUrl { get; set; }

        public static string SymbolServerUrl { get; set; }

        private static string symbolApiKey = null;
        public static string SymbolApiKey
        {
            get
            {
                if (symbolApiKey != null)
                    return symbolApiKey;

                return Environment.GetEnvironmentVariable("NugetSymbolApiKey");
            }

            set { symbolApiKey = value; }
        }
       
        public static TimeSpan? Timeout { get; set; }

        private static string nugetExePath = null;
        public static string NuGetExePath
        {
            get
            {
                if (nugetExePath != null)
                    return nugetExePath;

                return PvcUtil.FindBinaryInPath("NuGet.exe", "NuGet.bat");
            }
            set
            {
                nugetExePath = value;
            }
        }

    }
}
