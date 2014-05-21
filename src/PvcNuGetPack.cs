using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Edokan.KaiZen.Colors;
using PvcCore;

namespace PvcPlugins
{
    public class PvcNuGetPack : PvcPlugin
    {
        private readonly string basePath = null;
        private readonly string version = null;
        private readonly string exclude = null;
        private readonly bool defaultExcludes = true;
        private readonly bool packageAnalysis = true;
        private readonly bool includeReferencedProjects = true;
        private readonly bool excludeEmptyDirectories = true;
        private readonly bool createSymbolsPackage = false;
        private readonly bool buildProject = false;
        private readonly string minClientVersion = null;
        private readonly string properties = null;

        public PvcNuGetPack(
            string basePath = null,
            string version = null,
            string exclude = null,
            bool defaultExcludes = true,
            bool packageAnalysis = true,
            bool includeReferencedProjects = false,
            bool excludeEmptyDirectories = false,
            bool createSymbolsPackage = false,
            bool buildProject = true,
            string minClientVersion = null,
            string properties = null
            )
        {
            this.basePath = basePath;
            this.version = version;
            this.exclude = exclude;
            this.defaultExcludes = defaultExcludes;
            this.packageAnalysis = packageAnalysis;
            this.includeReferencedProjects = includeReferencedProjects;
            this.excludeEmptyDirectories = excludeEmptyDirectories;
            this.createSymbolsPackage = createSymbolsPackage;
            this.buildProject = buildProject;
            this.minClientVersion = minClientVersion;
            this.properties = properties;
        }

        public override IEnumerable<PvcStream> Execute(IEnumerable<PvcStream> inputStreams)
        {
            var nuspecAndProjectStreams = inputStreams.Where(x => Regex.IsMatch(x.StreamName, @"\.(.*proj|nuspec)$"));

            var nupkgFiles = new List<string>();
            var nupkgFilesCreated = 0;
            FileSystemWatcher watcher = new FileSystemWatcher(Directory.GetCurrentDirectory(), "*.nupkg");
            FileSystemEventHandler eventHandler = (object s, FileSystemEventArgs e) =>
            {
                if (!nupkgFiles.Contains(e.FullPath))
                    nupkgFiles.Add(e.FullPath);
            };

            watcher.Created += eventHandler;
            watcher.Changed += eventHandler;

            watcher.EnableRaisingEvents = true;

            foreach (var nuStream in nuspecAndProjectStreams)
            {
                var args = new List<string>(new[]{
                    "pack",
                    nuStream.StreamName
                });

                if (this.basePath != null) args.AddRange(new[] { "-BasePath", this.basePath });
                if (this.version != null) args.AddRange(new[] { "-Version", this.version });
                if (this.exclude != null) args.AddRange(new[] { "-Exclude", this.exclude });
                if (this.defaultExcludes == false) args.Add("-NoDefaultExcludes");
                if (this.packageAnalysis == false) args.Add("-NoPackageAnalysis");
                if (this.includeReferencedProjects) args.Add("-IncludeReferencedProjects");
                if (this.excludeEmptyDirectories) args.Add("-ExcludeEmptyDirectories");
                if (this.createSymbolsPackage) args.Add("-Symbols");
                if (this.buildProject) args.Add("-Build");
                if (this.minClientVersion != null) args.AddRange(new[] { "-MinClientVersion", this.minClientVersion });
                if (this.properties != null) args.AddRange(new[] { "-Properties", this.properties });
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
                {
                    throw new PvcException("NuGet Pack Error: " + errOutput);
                }
                else
                {
                    nupkgFilesCreated++;
                }
            }

            // wait for the watch to catch all the files created
            while (nupkgFiles.Count != (nupkgFilesCreated * (this.createSymbolsPackage ? 2 : 1))) { }

            var nupkgStreams = nupkgFiles.Select(x => new PvcStream(new FileStream(x, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)).As(x));
            return inputStreams.Except(nuspecAndProjectStreams).Concat(nupkgStreams);
        }
    }
}
