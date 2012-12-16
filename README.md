# Windows Azure Storage Extensions

Storage Extensions library provides generic `TableSet<TEntity>` context for managing and querying entities from [Windows Azure Storage Tables](http://blogs.msdn.com/b/windowsazurestorage/archive/2012/11/06/windows-azure-storage-client-library-2-0-tables-deep-dive.aspx).
It's built on top of the **Windows Azure Storage Client Library 2.0** and has next features:

**Using POCO entities**

Classes can use `PartitionKey` and `RowKey` attributes for defining composite table key.

**Table entities management**

  * Synchronous
      * Add()
      * Update()
      * Remove()
  * Asynchronous (TPL)
      * AddAsync()
      * UpdateAsync()
      * RemoveAsync()

**Table queries**

  * TableSet implements IQueryable interface for using Linq Expressions
     * Where()
     * Take()
  * Asynchronous queries by using Linq extensions (TPL)
     * ToListAsync()

## Download

### Via Git
To get the source code of the library via git just type:

```git
git clone git://github.com/dtretyakov/WindowsAzure.git
cd ./WindowsAzure
```

### Via NuGet
To install library by using Nuget package manager execute next command:

```
Install-Package WindowsAzure.StorageExtensions -Pre
```

## Dependencies
Storage Extensions requires .NET Framework 4.5 and [WindowsAzure.Storage](https://nuget.org/packages/WindowsAzure.Storage) nuget package.

## Code Samples

Declaring a new POCO class:

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

Creating a new table context:

```csharp
var tableClient = CloudStorageAccount.DevelopmentStorageAccount;
var countryTable = new TableSet<Country>(tableClient);
```

Adding a new entity:

```csharp
var resultSync = tableSet.Add(country);
var resultAsync = await tableSet.AddAsync(country);
```

Updating an entity:

```csharp
resultSync.Area += 333333;
resultSync = tableSet.Update(resultSync);

resultAsync.Population *= 2;
resultAsync = await tableSet.UpdateAsync(resultAsync);
```

Removing entities:

```csharp
tableSet.Remove(resultSync);
await tableSet.RemoveAsync(resultAsync);
```

Querying entities:

```csharp
var query = tableSet.Where(
        p => p.Formed > new DateTime(1950, 1, 1) &&
             (p.PresidentsCount < 10 ||
              p.Population < 10000000 && p.PresidentsCount > 10 && p.IsExists));

var resultsSync = query.ToList();
var resultsAsync = await query.ToListAsync();
```
