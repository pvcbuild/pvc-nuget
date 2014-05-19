pvc-nuget
=========

PVC NuGet Plugin to generate NuGet packages/symbols and push them to NuGet.org and other compatible servers.

Examples:

```
pvc.Source("**/*.nuspec", "OtherProject.csproj")
   .Pipe(new PvcNuGetPack())
   .Pipe(new PvcNuGetPush());
```

####PvcNuGetPack Configuration Options
The following options are available as optional/named parameters of the constructor:

**basePath** The base path of the files defined in the nuspec file.

**version** Overrides the version number from the nuspec file.

**exclude** Specifies one or more wildcard patterns to exclude when creating a package.

**defaultExcludes** Exclude NuGet package files and files and folders starting with a dot e.g. .svn.

**packageAnalysis** Run package analysis after building the package.

**includeReferencedProjects** Include referenced projects either as dependencies or as part of the package. If a referenced project has a corresponding nuspec file that has the same name as the project, then that referenced project is added as a dependency. Otherwise, the referenced project is added as part of the package.

**excludeEmptyDirectories** Exclude empty directories when building the package.

**createSymbolsPackage** Determines if a package containing sources and symbols should be created.

**buildProject** Determines if the project should be built before building the package.

**minClientVersion** Set the minClientVersion attribute for the created package. This value will override the value of the existing minClientVersion attribute (if any) in the .nuspec file.

**properties** Provides the ability to specify a semicolon ";" delimited list of properties when creating a package.

```
pvc.Source("**/*.nuspec")
   .Pipe(new PvcNuGetPack(
        createSymbolsPackage: true
   ));
```

####PvcNuGetPush Configuration Options

The following options are available as optional/named parameters of the constructor:

**serverUrl** Specifies the server URL. Can be set via static configuration value `PvcNuGet.ServerUrl`. If not specified, nuget.org is used unless DefaultPushSource config value is set in the NuGet config file. 

**apiKey** The API key for the server. Can be set via static configuration value `PvcNuGet.ApiKey`.

**symbolServerUrl** Specifies the server URL for symbols. If not specified, SymbolSource.org is used. Can be set via static configuration value `PvcNuGet.SymbolServerUrl`.

**symbolApiKey** Alternate API key for the symbol server. If not specified, `apiKey` will be used. Can be set via static configuration value `PvcNuGet.SymbolApiKey`.

**timeout** Specifies the timeout for pushing to a server in seconds. Defaults to 300 seconds (5 minutes).


```
pvc.Source("**/*.nuspec")
   .Pipe(new PvcNuGetPack())
   .Pipe(new PvcNuGetPush(
        timeout: TimeSpan.FromMinutes(10)
   ));
```