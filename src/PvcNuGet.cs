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
        public static string ApiKey { get; set; }

        public static string ServerUrl { get; set; }

        public static string SymbolServerUrl { get; set; }

        public static string SymbolApiKey { get; set; }
        
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
