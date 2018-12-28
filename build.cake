var target = Argument("target", "Pull-Request");
var configuration = Argument("configuration", "Release");
var verbosity = Argument<DotNetCoreVerbosity>("verbosity", DotNetCoreVerbosity.Quiet);
var artifactsPath = Argument("artifactsPath", "./artifacts");
var solutionFile = "Voltmeter.sln";
var testUnitProjectPattern = "./**/*Tests.Unit.csproj";
var testIntegrationProjectPattern = "./**/*.Tests.Integration.csproj";
var testAcceptanceProjectPattern = "./**/*.Tests.Acceptance.csproj";
var isTfsBuild = (EnvironmentVariable("TF_BUILD") ?? "False").ToLower() == "true";
var testLogger = isTfsBuild ? "trx" : "console;verbosity=normal";

void RunTests(string projectGlob)
{
  var settings = new DotNetCoreTestSettings
    {
      Configuration = configuration,
      Verbosity = verbosity,
      NoRestore = true,
      NoBuild = true,
      Logger = testLogger,
      ArgumentCustomization = args => 
        args
        .Append("/p:CollectCoverage=true")
        .Append("/p:CoverletOutputFormat=cobertura")
    };

    var testProjects = GetFiles(projectGlob);

    foreach(var project in testProjects)
    {
      DotNetCoreTest(project.ToString(), settings);
    }
}

string GetOutputOfCommand(string command, string arguments)
{
	using(var process = StartAndReturnProcess(command, new ProcessSettings { Arguments = arguments, RedirectStandardOutput = true }))
	{
		process.WaitForExit();
			
		if(process.GetExitCode() != 0)
		{
			throw new Exception(string.Format("Could not execute `{0} {1}`", command, arguments));
		}

		return process.GetStandardOutput().Last();
	}
}

Task("Clean")
  .Does(() => {
	var settings = new DotNetCoreCleanSettings
	{
		Configuration = configuration,
		Verbosity = verbosity
	};

	DotNetCoreClean("", settings);

	var directoriesToDelete = new DirectoryPath[]{
		Directory(artifactsPath),
		Directory("publish")
	};
	
	CleanDirectories(directoriesToDelete);
});

Task("Restore")
  .Does(() => {
    var settings = new DotNetCoreRestoreSettings
    {
      Verbosity = verbosity 
    };

    DotNetCoreRestore("", settings);
});

Task("Build")
  .Does(() => {
	var buildNumber = EnvironmentVariable("Build.BuildNumber") ?? "1.0";	
	var sourceVersion = GetOutputOfCommand("git", "rev-parse HEAD");

	var settings = new DotNetCoreBuildSettings
	{
		Configuration = configuration,
		Verbosity = verbosity,
		NoRestore = true,
		ArgumentCustomization = args => 
			args
			.Append(string.Format("/p:InformationalVersion=\"{0}\"+g{1}\"", buildNumber, sourceVersion))
	};

	DotNetCoreBuild(solutionFile, settings);
});

Task("Package")
	.IsDependentOn("Package-Prerequisites")
	.IsDependentOn("Package-Api");

Task("Package-Prerequisites")
	.Does(() => {
	if(!DirectoryExists(artifactsPath))
	{
		CreateDirectory(artifactsPath);
	}
});

Task("Package-Api")
	.Does(() => {
	var settings = new DotNetCorePublishSettings
	{
		Configuration = configuration,
		Verbosity = verbosity,
		NoRestore = true,
		NoBuild = true,
		SelfContained = false
	};

	var apiProjects = GetFiles("./src/**/*.UI.csproj");

	foreach(var project in apiProjects)
	{
		// Publish each API project into its own folder
		settings.OutputDirectory = "./publish/" + project.GetFilenameWithoutExtension();

		DotNetCorePublish(project.ToString(), settings);

		// Azure expects API's to be zipped
		Zip(settings.OutputDirectory, artifactsPath + "/" + project.GetFilenameWithoutExtension() + ".zip");
	}
});

Task("Test-Unit")
  .Does(() => {
    RunTests(testUnitProjectPattern);
});

Task("Test-Integration")
  .Does(() => {
    RunTests(testIntegrationProjectPattern);
});

Task("Test-Acceptance")
  .Does(() => {
    RunTests(testAcceptanceProjectPattern);
});

//// Cake script Demands ////
// These targets are used to ensure we can succesfully
// build, package and test this repository.
// Here we check for specific versions of tools not controlled
// by the Cake script such as dotnet CLI for example.
Task("Demand-NetCoreSdk21")
	.Does(() => {
	var version = GetOutputOfCommand("dotnet", "--version");

	if(!version.Trim().StartsWith("2.1."))
	{
		throw new Exception("Expected dotnet SDK version 2.1.* but got " + version + " and I refuse to work now");
	}
});

//// Composite targets ////
// These targets don't do work themselves but chain several of
// the other targets together.
Task("Pull-Request")
	.IsDependentOn("Demand-ForBuild")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Build")
	.IsDependentOn("Test-Unit");

Task("Build-Release")
	.IsDependentOn("Demand-ForBuild")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Build")
	.IsDependentOn("Test-Unit")
	.IsDependentOn("Package");

Task("Demand-ForBuild")
	.IsDependentOn("Demand-NetCoreSdk21");


//// Setup/Teardown magic ////
// This is used to provide more details to Azure DevOps build pipelines
TaskSetup(setupContext =>
{
	if(!isTfsBuild) {
		return;
	}
    Warning("##[section]Starting: " + setupContext.Task.Name);
});

TaskTeardown(teardownContext =>
{
	if(!isTfsBuild) {
		return;
	}
    Warning("##[section]Finishing: " + teardownContext.Task.Name);
});


// Cake script main entry point. The target argument should define 
// a default in order to make sure calling the script is safe.
RunTarget(target);