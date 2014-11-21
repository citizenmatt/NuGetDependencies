using System;
using System.IO;
using NuGet;

namespace NuGetDependencies
{
  class Program
  {
    static void Main(string[] args)
    {
        var repoLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "JetBrains", "Installations", "Packages");
        //var repoLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NuGet", "Cache");

        Console.WriteLine("Opening repo: {0}", repoLocation);
        var repository = new LocalPackageRepository(repoLocation);
        Console.WriteLine();

        //const string name = "Microsoft.Web.Xdt";
        //const string name = "AsyncBridge.Net35";
        //const string name = "JetBrains.DotCommon.Core";
        const string name = "TaskParallelLibrary";
        //const string name = "xunit.core";

        var package = repository.FindPackage(name);
        if (package == null)
            throw new InvalidOperationException("Can't find package!");

        // The assemblies to be referenced (e.g. any .dlls in any lib folders
        Console.WriteLine("Assemblies that can be referenced:");
        foreach (var assemblyReference in package.AssemblyReferences)
            Console.WriteLine("\t{0}", assemblyReference.Path);
        Console.WriteLine();

        // What frameworks are explicitly supported. These translate to folders under lib.
        // E.g. .NETFramework,Version=v3.5 means there's a lib/net35 folder
        // If the framework name isn't listed here, there isn't a folder for it
        // NuGet will fail to install if there isn't a matching folder. E.g. a .net 3.5 project
        // cannot add a reference to a package that has a lib/net40 folder
        // BUT, NuGet seems to work fine with the same major version, e.g. a .net 4.5 project
        // CAN add a reference to a package that only has a lib/net40 folder
        // This value can be an empty enumeration, which is the same as all files being directly under lib.
        Console.WriteLine("Supported frameworks:");
        foreach (var framework in package.GetSupportedFrameworks())
            Console.WriteLine("\t{0}", framework);
        Console.WriteLine();

        Console.WriteLine("Assemblies to reference from the GAC:");
        foreach (var assembly in package.FrameworkAssemblies)
            Console.WriteLine("\t{0}", assembly.AssemblyName);
        Console.WriteLine();

        // What assemblies should be explicitly added, grouped by target framework (which can be null)
        // Allows packaging several .dll files, but only referencing a subset
        Console.WriteLine("Explicit assembly references:");
        foreach (var assembly in package.PackageAssemblyReferences)
        {
            Console.WriteLine("\tTarget framework: {0}", assembly.TargetFramework != null ? assembly.TargetFramework.ToString() : "<none>");
            foreach (var reference in assembly.References)
                Console.WriteLine("\t{0}", reference);
            Console.WriteLine();
        }
        Console.WriteLine();

        // The packages this package depends on, grouped by target framework. If the dependencies aren't
        // grouped by target framework, there will be only one dependency set, with a null TargetFramework.
        // If there are multiple dependency sets and one contains a null target framework, then it is a fallback
        // group, and used if none of the other sets match
        Console.WriteLine("Depedency sets:");
        foreach (var set in package.DependencySets)
        {
            Console.WriteLine("\tTarget framework: {0}", set.TargetFramework != null ? set.TargetFramework.ToString() : "<none>");
            foreach (var dependency in set.Dependencies)
                Console.WriteLine("\tDependency: {0}", dependency);
            Console.WriteLine();
        }
        Console.WriteLine();
    }
  }
}
