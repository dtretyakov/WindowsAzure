# Windows Azure Storage Extensions

*Windows Azure Storage Extensions* is a .NET library aimed for managing and querying entities from [Windows Azure Storage Tables](http://msdn.microsoft.com/en-us/library/windowsazure/dd179463.aspx).

It's built on top of the **[Windows Azure Storage Client Library 2.0](https://github.com/WindowsAzure/azure-sdk-for-net)**, provides **async interfaces** ([Task-based Asynchronous Pattern](http://msdn.microsoft.com/en-us/library/hh873175.aspx)) and **LINQ to Azure Table** queries via `TableSet` context by using **POCO** entities.

Latest project build: <a href="http://teamcity.codebetter.com/viewType.html?buildTypeId=bt986&guest=1"><img src="http://teamcity.codebetter.com/app/rest/builds/buildType:(id:bt986)/statusIcon" alt=""/></a>

## Table Of Contents
* [Features](#features)
* [Download](#download)
* [Dependencies](#dependencies)
* [Code Samples](#code-samples)
* [Sponsors](#sponsors)

## Features

###POCO Objects

Entity's properties and fields should be marked by one or both of `PartitionKey` and `RowKey` attributes for defining composite table key.
Also can be used `Timestamp`, `ETag`, `Property` and `Ignore` attributes.

###Entities Management

Generic `TableSet` context provides a synchronous & asynchronous ([TAP](http://msdn.microsoft.com/en-us/library/hh873175.aspx)) methods for managing entities:

  * *Synchronous*: Add, AddOrUpdate, Update and Remove.
  * *Asynchronous*: AddAsync, AddOrUpdateAsync, UpdateAsync and RemoveAsync.

To avoid [restrictions of group operations](http://msdn.microsoft.com/en-us/library/windowsazure/dd894038.aspx) in Azure Storage Table we can control request execution via TableSet's `Configuration.PartitioningMode` property.

<table>
  <thead>
    <tr><th>Mode</th><th>Partitioning</th><th>Request Execution</th></tr>
  </thead>
  <tbody>
    <tr><td>Sequential</td><td>Yes</td><td>Sequential</td></tr>
    <tr><td>Parallel</td><td>Yes</td><td>Parallel</td></tr>
    <tr><td>None</td><td>No</td><td>Sequential</td></tr>
  </tbody>
</table>

Default PartitioningMode is Sequential.

###LINQ Queries

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

**NOTE**: For creating a custom queries you should take a look at next article [Mixing LINQ Providers and LINQ to Objects](http://msdn.microsoft.com/en-us/vstudio/ff963710.aspx). 

###Asynchronous LINQ Queries

In addition `TableSet` can be used for **asynchronous queries** powered by LINQ extensions (TAP) in [EF 6 Async style](http://weblogs.asp.net/scottgu/archive/2012/12/11/entity-framework-6-alpha2-now-available.aspx).

Available methods:
* FirstAsync
* FirstOrDefaultAsync
* SingleAsync
* SingleOrDefaultAsync
* TakeAsync
* ToListAsync

###LINQ Projections

LINQ Projections supported with a limitation - projection class should be a reference type.

###TAP-based Extensions

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

###Task Cancellation

All of TAP-based methods accepts optional `CancellationToken` parameter for [Task Cancellation](http://msdn.microsoft.com/en-us/library/dd997396.aspx).

## Download

### Via NuGet
To install library by using [Windows Azure Storage Extensions](https://nuget.org/packages/WindowsAzure.StorageExtensions/) nuget package execute next command:

```
Install-Package WindowsAzure.StorageExtensions
```

### Via Git
To get the source code of the library via git just type:

```git
git clone git://github.com/dtretyakov/WindowsAzure.git
cd ./WindowsAzure
```

## Dependencies
Storage Extensions requires .NET Framework 4.0 or higher and [WindowsAzure.Storage](https://nuget.org/packages/WindowsAzure.Storage) nuget package.

## Code Samples

* Declaring a new POCO class:

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

## Sponsors
* [JetBrains](http://www.jetbrains.com): [ReSharper](http://www.jetbrains.com/resharper/) and [TeamCity](http://www.jetbrains.com/teamcity/).
* [CodeBetter](http://codebetter.com): [CodeBetter CI Server](http://codebetter.com/codebetter-ci/).
