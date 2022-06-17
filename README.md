# Windows Azure Storage Extensions

## Info
| Version | Dependencies | Information 
| :--- | :--- | :---
[![WindowsAzure.StorageExtensions](https://buildstats.info/nuget/WindowsAzure.StorageExtensions)](https://www.nuget.org/packages/WindowsAzure.StorageExtensions) | [Windows.Azure.Storage](https://www.nuget.org/packages/WindowsAzure.Storage/) | This dependency is declared deprecated by Microsoft.
[![MicrosoftAzureCosmos.TableExtensions](https://buildstats.info/nuget/MicrosoftAzureCosmosTable.Extensions)](https://www.nuget.org/packages/MicrosoftAzureCosmosTable.Extensions) | [Microsoft.Azure.Cosmos.Table](https://www.nuget.org/packages/Microsoft.Azure.Cosmos.Table/1.0.7) |
[![V2-preview](https://img.shields.io/badge/nuget-v2.0.0_preview_00-blue)](https://www.nuget.org/packages/MicrosoftAzureCosmosTable.Extensions) | [Microsoft.Azure.Cosmos.Table - Preview](https://www.nuget.org/packages/Microsoft.Azure.Cosmos.Table/2.0.0-preview) | This was a preview version, but now declared deprecated by Microsoft.

*Windows Azure Storage Extensions* is a .NET library aimed at managing and querying entities from [Azure Storage Tables](http://msdn.microsoft.com/en-us/library/windowsazure/dd179463.aspx).

It's built on top of the **[Azure .NET SDK](https://github.com/WindowsAzure/azure-sdk-for-net)**, provides **async interfaces** ([Task-based Asynchronous Pattern](http://msdn.microsoft.com/en-us/library/hh873175.aspx)) and **LINQ to Azure Table** queries via `TableSet` context by using **POCO** entities.

## Table Of Contents
* [Features](#features)
* [Code Samples](#code-samples)
* [Contributors](#Contributors)

## Features

### Attribute Mapping

POCO properties and fields should be marked by one or both of `PartitionKey` and `RowKey` attributes for defining composite table key. Also can be used `Timestamp`, `ETag`, `Property` and `Ignore` attributes.

### Fluent Mapping

Fluent mapping is the namesake mapping style that we use as an alternative to the AttributeMapping. It's a fluent interface that allows you to map your entities completely in code, with all the compile-time safety and refactorability that brings.

#### EntityTypeMap class

<code>EntityTypeMap<T></code> class is the basis of all your mappings, you derive from this to map anything.

``` c#
public class AddressMap : EntityTypeMap<Address>
{
  public AddressMap()
  {
      this.PartitionKey(p => p.CountryCode)
          .RowKey(p => p.Id)
          .Ignore(p => c.Country);
  }
}
```

You map your entities properties inside the constructor.

> **Syntax note:** Every mapping inside a <code>EntityTypeMap<T></code> is built using lambda expressions, which allow us to reference the properties on your entities without sacrificing compile-time safety. The lambdas typically take the form of <code>x => x.Property</code>. The <code>x</code> on the left is the parameter declaration, which will implicitly be of the same type as the entity being mapped, while the <code>x.Property</code> is accessing a property on your entity (coincidentally called "Property" in this case).

Once you've declared your <code>EntityTypeMap<T></code> you're going to need to map the properties on your entity. There are several methods available that map your properties in different ways, and each one of those is a [chainable method](http://martinfowler.com/dslwip/MethodChaining.html) that you can use to customise the individual mapping.

##### PartitionKey and RowKey

Every mapping requires an `PartitionKey` and `RowKey` of some kind.

The PartitionKey is mapped using the <code>PartitionKey</code> method, which takes a lambda expression that accesses the property on your entity that will be used for the PartitionKey.

    PartitionKey(x => x.MyPartitionKeyProperty);
    
The PartitionKey is mapped using the <code>RowKey</code> method, which takes a lambda expression that accesses the property on your entity that will be used for the PartitionKey.

    RowKey(x => x.MyRowKeyProperty);

##### Ignore

If you need to ignore a property by using the <code>Ignore</code> method, which takes a lambda expression that accesses the property on your entity that will ignored by the serializer.

    Ignore(x => x.MyPropertyToIgnore);

##### ETag and Timestamp

The `ETag` property is mapped using the methods <code>ETag</code>, which takes a lambda expression that accesses the property on your entity that will be mapped.

    ETag(x => x.MyETagProperty);

The `Timestamp` property is mapped using the methods <code>Timestamp</code>, which takes a lambda expression that accesses the property on your entity that will be mapped.

    Timestamp(x => x.MyTimestampProperty);

#### Register custom mapping assembly 

The `TableEntityConverter` class will try to find the mapping classes for your entities on the same assembly of the entitie, if you are using a different assembly for mappings classes you'll need to register this assembly by using the <code>RegisterAssembly</code> method from class <code>EntityTypeMap</code>. **ATTENTION: this method just need to be called once and before instantiating the <code>TableSet</code> class**. Below there is a sample of how we call the 

    EntityTypeMap.RegisterAssembly(typeof(MyEntity).Assembly)

### Entities Management

Generic `TableSet` context provides a synchronous & asynchronous ([TAP](http://msdn.microsoft.com/en-us/library/hh873175.aspx)) methods for managing entities:

  * *Synchronous*: Add, AddOrUpdate, Update and Remove.
  * *Asynchronous*: AddAsync, AddOrUpdateAsync, UpdateAsync and RemoveAsync.

To avoid [restrictions of group operations](http://msdn.microsoft.com/en-us/library/windowsazure/dd894038.aspx) in Azure Storage all entities sorted by partition keys and merged into groups by 100 entities. Execution of requests with such batch operations can be configured via TableSet's `ExecutionMode` property. Allowed values:
* Sequential
* Parallel

Default ExecutionMode is Sequential.

### LINQ Queries

`TableSet` context implements `IQueryable` interface for using [LINQ Expressions](http://msdn.microsoft.com/en-us/library/vstudio/bb397926.aspx). Provider supports next synchronous LINQ methods:
* First
* FirstOrDefault
* Single
* SingleOrDefault
* Take
* Where

To utilize [filtering capabilities of string properties](http://msdn.microsoft.com/en-us/library/windowsazure/dd894031.aspx) it supports:
* [Compare](http://msdn.microsoft.com/en-us/library/84787k22.aspx)
* [CompareTo](http://msdn.microsoft.com/en-us/library/fkw3h78a.aspx)
* [CompareOrdinal](http://msdn.microsoft.com/en-us/library/af26w0wa.aspx)

Also you can use [**Contains** method](http://msdn.microsoft.com/en-us/library/ms132407.aspx). In this case query statement for each collection's item will be joined by using OData _or_ operator.

**NOTE**: For creating a custom queries you should take a look at next article: [Mixing LINQ Providers and LINQ to Objects](http://msdn.microsoft.com/en-us/vstudio/ff963710.aspx). 

### Asynchronous LINQ Queries

In addition `TableSet` can be used for **asynchronous queries** powered by LINQ extensions (TAP) in [EF 6 Async style](http://weblogs.asp.net/scottgu/archive/2012/12/11/entity-framework-6-alpha2-now-available.aspx).

Available methods:
* FirstAsync
* FirstOrDefaultAsync
* SingleAsync
* SingleOrDefaultAsync
* TakeAsync
* ToListAsync

### LINQ Projections

LINQ Projections supported with a limitation - projection class should be a reference type.

### TAP-based Extensions

Library contains TAP-based extensions for following Azure Storage Library classes:
* CloudBlobClient
* CloudBlobContainer
* CloudTableClient
* CloudTable

To use it just add _Async_ postfix to synchronous method name for instance:

```csharp
blobs = cloudBlobContainer.ListBlobs();
blobs = await cloudBlobContainer.ListBlobsAsync();
```

### Task Cancellation

All of TAP-based methods accepts optional `CancellationToken` parameter for [Task Cancellation](http://msdn.microsoft.com/en-us/library/dd997396.aspx).

## Code Samples

* Declaring a new POCO class and using attribute mapping:

```csharp
public sealed class Country
{
    [PartitionKey]
    public string Continent { get; set; }

    [RowKey]
    public string Name { get; set; }

    public long Population { get; set; }
    public double Area { get; set; }
    public DateTime Formed { get; set; }
}
```


* Declaring a new POCO class and using fluent mapping:

```csharp
public sealed class Country
{
    public string Continent { get; set; }
    public string Name { get; set; }
    public long Population { get; set; }
    public double Area { get; set; }
    public DateTime Formed { get; set; }
}

public class CountryMapping : WindowsAzure.Table.EntityConverters.TypeData.EntityTypeMap<Country>
{
    public CountryMapping() {
        this.PartitionKey(p => p.Continent)
            .RowKey(p => p.Name);
    }
}
```

* Creating a new table context:

```csharp
var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
var tableClient = storageAccount.CreateCloudTableClient();

var countryTable = new TableSet<Country>(tableClient);
```

* Adding a new entity:

```csharp
var resultSync = countryTable.Add(country);
var resultAsync = await countryTable.AddAsync(country);
```

* Updating an entity:

```csharp
resultSync.Area += 333333;
resultSync = countryTable.Update(resultSync);

resultAsync.Population *= 2;
resultAsync = await countryTable.UpdateAsync(resultAsync);
```

* Removing entities:

```csharp
countryTable.Remove(resultSync);
await countryTable.RemoveAsync(resultAsync);
```

* Querying entities:

```csharp
var query = countryTable.Where(
        p => p.Formed > new DateTime(1950, 1, 1) &&
             (p.PresidentsCount < 10 ||
              p.Population < 10000000 && p.PresidentsCount > 10 && p.IsExists));

resultsSync = query.ToList();
resultsAsync = await query.ToListAsync();
```

* Using LINQ projections:

```csharp
var projection = from country in countryTable
                 where country.Area > 400000
                 select new { country.Continent, country.Name };

var result = projection.ToList();
result = await projection.ToListAsync();
```

* Using Contains in the LINQ query:

```csharp
var countryNames = new List<string> { "Germany", "Finland" };
var countries = countryTable.Where(p => countryNames.Contains(p.Name)).ToList();
```

## Build script

We create a build script using [Cake](https://cakebuild.net).

### Files of interest

We need somes files for a build script work.

- `build.ps1`, `build.sh` and `build.cmd` (an alias to `build.ps1`)

These are bootstrapper scripts that ensure you have Cake and other required dependencies installed. The bootstrapper scripts are also responsible for invoking Cake. These files are optional, and not a hard requirement. If you would prefer not to use these scripts you can invoke Cake directly from the command line, once you have downloaded and extracted it.

- `tools/` folder and `tools/packages.config`

This is the package configuration that tells the bootstrapper script what NuGet packages to install in the tools folder. An example of this is Cake itself or additional tools such as unit test runners, ILMerge etc.

- `build.cake`

This is the actual build script, it doesn't have to be named this but this will be found by default and we keeped. Our build script executes on base of two list of projects:
- List of projects to build and when needed package and publish
- List of projects to build, test and when needed collect code coverage

Because of that the build needs two main [file glob arguments](https://en.wikipedia.org/wiki/Glob_(programming)), that are used to find the projects that matches this glob pattern, more about the argument bellow.

### Build Script Arguments

More arguments can be found on the `build.cake` script.

- `--Project` file glob pattern for the projects to build, package and publish.
- `--Tests` file glob pattern for the projects to build, test and collect code coverage.
- `--Target` defines the actions/task to be executed.
- `--Configuration` defines the build configuration to be used on projects.
- `--PackageVersion` defines the version to bump into the project before build, package and publish
- `--NugetSource` nuget source api URL
- `--NugetApiKey` nuget source api key

#### Build samples

This sample we'll only build the foundation project.

For Windows

` .\build.cmd --Target=Build --Projects="./WindowsAzure/*.csproj"`

For MacOS / Linux

` .\build.sh --Target=Build --Projects="./WindowsAzure/*.csproj"`

### Build and Test sample

This sample we'll only build the foundation project.

For Windows

` .\build.cmd --Target=Test --Projects="./WindowsAzure/*.csproj" --Tests="./WindowsAzure.Tests/*.csproj"`

For MacOS / Linux

` .\build.sh --Target=Test --Projects="./WindowsAzure/*.csproj" --Tests="./WindowsAzure.Tests/*.csproj"`

### Build, Test and Collect Code Coverage sample

This sample we'll only build the foundation project.

For Windows

` .\build.cmd --Target=TestCoverage --Projects="./WindowsAzure/*.csproj" --Tests="./WindowsAzure.Tests/*.csproj"`

For MacOS / Linux

` .\build.sh --Target=TestCoverage --Projects="./WindowsAzure/*.csproj" --Tests="./WindowsAzure.Tests/*.csproj"`

### Build, Test and Publish to Visual Studio NuGet

This sample we'll only build the foundation project.

For Windows

` .\build.cmd --Target=Publish --PackageVersion=1.6.0 --Projects="./WindowsAzure/*.csproj" --Tests="./WindowsAzure.Tests/*.csproj" --NugetApiKey={the nuget apikey come here}`

For MacOS / Linux

` .\build.sh --Target=Publish --PackageVersion=1.6.0 --Projects="./WindowsAzure/*.csproj" --Tests="./WindowsAzure.Tests/*.csproj" --NugetApiKey={the nuget apikey come here}`

## Contributors
Great thanks to all of projects contributors. 
* [Dmitry Tretyakov](dtretyakov) for project idea.
* [Timothy Makarov](timothy-makarov) for performance optimizations.
* [Gabriel Marquez](gblmarquez) for Fluent API mapping.
* [Sebastian Betzin](sbetzin) for additional TableSet methods.
* [Tom Dietrich](tdietrich513) for his contributions.
* [Stef Heyenrath](StefH) for his contributions related to Microsoft.Azure.Cosmos.Table.
* And [Community](https://github.com/dtretyakov/WindowsAzure/issues) for a valuable feedback.

See complete list of [project contributors](https://github.com/dtretyakov/WindowsAzure/graphs/contributors).

We appreciate all kinds of feedback, so please feel free to send a [PR](https://github.com/dtretyakov/WindowsAzure/pulls) or write an [issue](https://github.com/dtretyakov/WindowsAzure/issues).
