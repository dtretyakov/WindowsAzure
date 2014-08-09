# Windows Azure Storage Extensions

*Windows Azure Storage Extensions* is a .NET library aimed at managing and querying entities from [Windows Azure Storage Tables](http://msdn.microsoft.com/en-us/library/windowsazure/dd179463.aspx).

It's built on top of the **[Windows Azure Storage Client Library 2.0](https://github.com/WindowsAzure/azure-sdk-for-net)**, provides **async interfaces** ([Task-based Asynchronous Pattern](http://msdn.microsoft.com/en-us/library/hh873175.aspx)) and **LINQ to Azure Table** queries via `TableSet` context by using **POCO** entities.

Latest project build: <a href="http://teamcity.codebetter.com/viewType.html?buildTypeId=bt986&guest=1"><img src="http://teamcity.codebetter.com/app/rest/builds/buildType:(id:bt986)/statusIcon" alt=""/></a>

## Table Of Contents
* [Features](#features)
* [Download](#download)
* [Dependencies](#dependencies)
* [Code Samples](#code-samples)
* [Sponsors](#sponsors)

## Features

### POCO Objects

Entity's properties and fields should be marked by one or both of `PartitionKey` and `RowKey` attributes for defining composite table key.
Also can be used `Timestamp`, `ETag`, `Property` and `Ignore` attributes.

### Fluent mapping

Fluent mapping is the namesake mapping style that we use as an alternative to the AttributeMapping. It's a fluent interface that allows you to map your entities completely in code, with all the compile-time safety and refactorability that brings.

#### EntityTypeMap class

<code>EntityTypeMap<T></code> class is the basis of all your mappings, you derive from this to map anything.

    public class AddressMap : EntityTypeMap<Address>
    {
      public AddressMap()
      {
          this.PartitionKey(p => p.CountryCode)
              .RowKey(p => p.Id)
              .Ignore(p => c.Country);
      }
    }

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

###Entities Management

Generic `TableSet` context provides a synchronous & asynchronous ([TAP](http://msdn.microsoft.com/en-us/library/hh873175.aspx)) methods for managing entities:

  * *Synchronous*: Add, AddOrUpdate, Update and Remove.
  * *Asynchronous*: AddAsync, AddOrUpdateAsync, UpdateAsync and RemoveAsync.

To avoid [restrictions of group operations](http://msdn.microsoft.com/en-us/library/windowsazure/dd894038.aspx) in Azure Storage all entities sorted by partition keys and merged into groups by 100 entities. Execution of requests with such batch operations can be configured via TableSet's `ExecutionMode` property. Allowed values:
* Sequential
* Parallel

Default ExecutionMode is Sequential.

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

Also you can use [**Contains** method](http://msdn.microsoft.com/en-us/library/ms132407.aspx). In this case query statement for each collection's item will be joined by using OData _or_ operator.

**NOTE**: For creating a custom queries you should take a look at next article: [Mixing LINQ Providers and LINQ to Objects](http://msdn.microsoft.com/en-us/vstudio/ff963710.aspx). 

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

## Sponsors
* [JetBrains](http://www.jetbrains.com): [ReSharper](http://www.jetbrains.com/resharper/) and [TeamCity](http://www.jetbrains.com/teamcity/).
* [CodeBetter](http://codebetter.com): [CodeBetter CI Server](http://codebetter.com/codebetter-ci/).
