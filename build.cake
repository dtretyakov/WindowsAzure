///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
/// --Project
/// file glob pattern for the projects to build, package and publish.
var projectsGlob = Argument<string>("Projects", "./WindowsAzure/*.csproj");

/// --Tests
/// file glob pattern for the projects to build, test and collect code coverage.
var testsGlob = Argument<string>("Tests", "./WindowsAzure.Tests/*.csproj");

/// --Target
/// defines the actions/task to be executed.
var target = Argument<string>("Target", "Default");

/// --Configuration
/// defines the build configuration to be used on projects.
var configuration = Argument("Configuration", "Release");

/// --PackageVersion
/// defines the version to bump into the project before build, package and publish
var packageVersion = Argument<string>("PackageVersion", "");

/// --NugetSource
/// nuget source api URL
var nugetSource = Argument<string>("NugetSource", EnvironmentVariable("CAKE_NUGET_SOURCE") ?? "https://api.nuget.org/v3/index.json");

/// --NugetApiKey
/// nuget source api key
var nugetApiKey = Argument<string>("NugetApiKey", EnvironmentVariable("CAKE_NUGET_APIKEY") ?? EnvironmentVariable("SYSTEM_ACCESSTOKEN") ?? "");

/// --SkipClean
/// Skips the clean of folder and files
var skipClean = Argument<bool>("skipClean", false);

/// --SkipTests
/// Skips the build and tests projects
var skipTests = Argument<bool>("skipTests", false);

//////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&version=5.3.4"
#tool "nuget:?package=ReportGenerator&version=4.5.8"
#tool "nuget:?package=xunit.runner.console&version=2.4.1"

#addin "nuget:?package=Cake.Git&version=0.21.0"
#addin "nuget:?package=Cake.Coverlet&version=2.4.2"

//////////////////////////////////////////////////////////////////////
// EXTERNAL SCRIPTS
//////////////////////////////////////////////////////////////////////

// #load "./build/parameters.cake"

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

// Constants
const string NuGetOrgApi = "api.nuget.org";
var lastCommit = GitLogTip(".");

// Files
var csProjectFiles = GetFiles(projectsGlob, new GlobberSettings { IsCaseSensitive = true, Predicate = f => !f.Path.FullPath.ToLower().Contains("entityframework") });
var csTestProjectFiles = GetFiles(testsGlob, new GlobberSettings { IsCaseSensitive = true, Predicate = f => !f.Path.FullPath.ToLower().Contains("entityframework") });

// Directories
var nuget = Directory(".nuget");
var output = Directory("bld");
var outputPackagesDir = output + Directory("packages");
var outputTestResultsDir = output + Directory("testResults"); 
var outputCoverageTestResultsDir = output + Directory("coverageResults"); 

///////////////////////////////////////////////////////////////////////////////
//                          SETUP / TEARDOWN                                 //
///////////////////////////////////////////////////////////////////////////////
Setup(context => {
  Environment.SetEnvironmentVariable("DOTNET_CLI_TELEMETRY_OPTOUT", "1");
  Environment.SetEnvironmentVariable("DOTNET_SKIP_FIRST_TIME_EXPERIENCE", "1");
  Environment.SetEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en");

  Information($"Projects found in pattern \"{projectsGlob}\"");
  csProjectFiles
    .ToList()
    .ForEach(p => { Information("  {0}", p.ToString()); });

  if (skipTests == false) {
    Information($"Tests found in pattern \"{testsGlob}\"");
    csTestProjectFiles
      .ToList()
      .ForEach(p => { Information("  {0}", p.ToString()); });
  }

  Information("Executing tasks with arguments");
  Information("  WorkingDirectory {0}", Environment.CurrentDirectory);
  Information("  Target {0}", target);
  Information("  PackageVersion {0}", string.IsNullOrWhiteSpace(packageVersion) ? "is empty" : packageVersion);
  Information("  Configuration {0}", configuration);
  Information("  NuGetSource {0}", nugetSource);
  Information("  NuGetApiKey {0}", string.IsNullOrWhiteSpace(nugetApiKey) ? "is empty" : "is set (********)");
  Information("  SkipClean {0}", skipClean);
  Information("  SkipTests {0}", skipTests);

  Information(
    "Git commit {0}\n  Author: {1}\n  Date: {2:yyyy-MM-dd HH:mm:ss}\n\n  {3}\n",
    lastCommit.Sha, lastCommit.Author.Name, lastCommit.Author.When, lastCommit.MessageShort
  );
});

Teardown(context =>
{
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////

Task("Dummy")
  .Does(() => {});

Task("Clean")
  .Does(() => {
    // Clean artifact directories.
    CleanDirectories(new DirectoryPath[] { 
      output, outputPackagesDir, outputTestResultsDir, outputCoverageTestResultsDir
    });

    // Clean output directories.
    if (!skipClean) {
      CleanDirectories("./**/bin/" + configuration);
      CleanDirectories("./**/obj/" + configuration);
    }
  });

///////////////////////////////////////////////////////////////

Task("RestorePackage-NuGet")
  .Description("Restores dependencies")
  .Does(() => {
    DotNetCoreRestore(new DotNetCoreRestoreSettings
    {
        DisableParallel = false
    });
  });

///////////////////////////////////////////////////////////////

Task("Compile")
  .Description("Builds all projects")
  .IsDependentOn("Clean")
  .IsDependentOn("RestorePackage-NuGet")
  .IsDependentOn("Compile-Projects")
  .IsDependentOn("Compile-Tests")
  .Does(() => { });

Task("Compile-Projects")
  .Description("Builds libraries and apps projects")
  .WithCriteria(() => csProjectFiles.Count() > 0)
  .Does(() => {
    var settings = new DotNetCoreMSBuildSettings()
                    .HideLogo()
                    .SetMaxCpuCount(-1)
                    .SetConfiguration(configuration);

		foreach (var project in csProjectFiles) {
      DotNetCoreMSBuild(project.ToString(), settings);
    }
  });

Task("Compile-Tests")
  .Description("Builds tests projects")
  .WithCriteria(() => skipTests == false && csTestProjectFiles.Count() > 0)
  .Does(() => {
    var settings = new DotNetCoreMSBuildSettings()
                    .HideLogo()
                    .SetMaxCpuCount(-1)
                    .SetConfiguration(configuration);

		foreach (var project in csTestProjectFiles) {
      DotNetCoreMSBuild(project.ToString(), settings);
    }
  });

///////////////////////////////////////////////////////////////

Task("Test")
  .Description("Run tests on projects")
  .IsDependentOn("Compile")
  .WithCriteria(() => skipTests == false)
  .Does(() => {
		foreach (var projectFile in csTestProjectFiles) {
      var settings = new DotNetCoreTestSettings {
        NoBuild = true,
        NoRestore = true,
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("--results-directory " + outputTestResultsDir + File($"{projectFile.GetFilenameWithoutExtension()}-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}.xml"))
			};
      
			DotNetCoreTest(projectFile.FullPath, settings);
		}
  });

Task("CodeCoverage")
  .Description("Collects code coverage and generates report with results")
  .Does(() => {
    var testSettings = new DotNetCoreTestSettings {
      NoBuild = true,
      NoRestore = true,
      Configuration = configuration,
    };

    var coveletSettings = new CoverletSettings {
      CollectCoverage = true,
      Exclude = new List<string> { "[xunit.*]*" },
      CoverletOutputFormat = CoverletOutputFormat.opencover,
      CoverletOutputDirectory = outputCoverageTestResultsDir,
      CoverletOutputName = "" // will be defined later
    };

		foreach (var projectFile in csTestProjectFiles) {
      coveletSettings.CoverletOutputName = File($"{projectFile.GetFilenameWithoutExtension()}-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}.xml");
      DotNetCoreTest(projectFile, testSettings, coveletSettings);
    }
    
    // Upload a coverage report.
    ReportGenerator(GetFiles($"{outputCoverageTestResultsDir}/**/*.xml"), outputCoverageTestResultsDir + Directory("report"));    
  });

///////////////////////////////////////////////////////////////

Task("Package-NuGet")
  .Description("Generates NuGet packages for each project that contains a nuspec")
  .Does(() => {
    var settings = new DotNetCorePackSettings {
      Configuration = configuration,
      OutputDirectory = outputPackagesDir,
      ArgumentCustomization = args => args.Append("--include-symbols").Append("--serviceable").Append("--no-build")
    };
    
    foreach(var project in csProjectFiles) {
      DotNetCorePack(project.GetDirectory().FullPath, settings);
    }
  });

Task("Publish-NuGet")
  .Description("Pushes the nuget packages in the nuget folder to a NuGet source. Also publishes the packages into the feeds.")
  .Does(() => {
    // Make sure we have an API key.
    if (string.IsNullOrWhiteSpace(nugetApiKey)) {
      throw new CakeException("Aborting task! Argument \"--NuGetApiKey\" was not specified.");
    }

    // Upload every package to the provided NuGet source (defaults to nuget.org).
    var packages = GetFiles($"{outputPackagesDir.Path.FullPath}/*{packageVersion}.nupkg");

    foreach (var package in packages) {
      Verbose($"NuGet Push for \"{package.ToString()}\"");
      NuGetPush(package, new NuGetPushSettings {
        Verbosity = NuGetVerbosity.Detailed,
        Source = nugetSource,
        ApiKey = nugetApiKey
      });
    }
  });

///////////////////////////////////////////////////////////////

Task("Update-Version")
  .Does(() => {
    if (string.IsNullOrWhiteSpace(packageVersion)) {
      throw new CakeException("Aborting task! Argument \"--PackageVersion\" was not specified.");
    }

    foreach (var file in csProjectFiles) {
      Verbose($"Set version to {packageVersion} in project \"{file}\".");
      XmlPoke(file, "//PropertyGroup/VersionPrefix", packageVersion);
    }
  });

///////////////////////////////////////////////////////////////

Task("Publish")
  .IsDependentOn("Update-Version")
  .IsDependentOn("Compile")
  .IsDependentOn("Test")
  .IsDependentOn("Package-NuGet")
  .IsDependentOn("Publish-NuGet");
  
Task("TestCoverage")
  .IsDependentOn("Compile")
  .IsDependentOn("Test")
  .IsDependentOn("CodeCoverage");

Task("Build")
  .IsDependentOn("Compile");

Task("Default")
  .IsDependentOn("Compile")
  .IsDependentOn("Test");

RunTarget(target);