# 1.3.0 (22 February 2021)
- [#64](https://github.com/dtretyakov/WindowsAzure/pull/64) - Add support for .NET 4.6; Expose table name contributed by [gblmarquez](https://github.com/gblmarquez)
- [#65](https://github.com/dtretyakov/WindowsAzure/pull/65) - Add support to serialize/deserialize non supported properties or fields.  contributed by [artyomabrahamyan](https://github.com/artyomabrahamyan)
- [#69](https://github.com/dtretyakov/WindowsAzure/pull/69) - Update packages and Adds new build scripts contributed by [gblmarquez](https://github.com/gblmarquez)
- [#70](https://github.com/dtretyakov/WindowsAzure/pull/70) - Add support for Microsoft.Azure.Cosmos.Table contributed by [StefH](https://github.com/StefH)
- [#58](https://github.com/dtretyakov/WindowsAzure/issues/58) - Run on framework &quot;net46&quot; [enhancement]
- [#63](https://github.com/dtretyakov/WindowsAzure/issues/63) - Add support for Name [enhancement]

# 1.2.0 (26 March 2018)
- [#60](https://github.com/dtretyakov/WindowsAzure/pull/60) - set result of table query to return object (.NET Core configuration) contributed by [bluechipalex](https://github.com/bluechipalex)
- [#53](https://github.com/dtretyakov/WindowsAzure/issues/53) - .NET Core (.NET Standard) Support [enhancement]

# 1.1.0-beta1 (25 October 2016)
- [#56](https://github.com/dtretyakov/WindowsAzure/pull/56) - Upgrade to azure storage sdk 7 contributed by [dtretyakov](https://github.com/dtretyakov)
- [#57](https://github.com/dtretyakov/WindowsAzure/pull/57) - Add .net core support contributed by [dtretyakov](https://github.com/dtretyakov)

# 1.0.4 (29 September 2016)
- [#51](https://github.com/dtretyakov/WindowsAzure/pull/51) - #43 Implements support for enum properties contributed by [gblmarquez](https://github.com/gblmarquez)
- [#52](https://github.com/dtretyakov/WindowsAzure/pull/52) - Ignoring a property is not properly respected contributed by [gblmarquez](https://github.com/gblmarquez)
- [#43](https://github.com/dtretyakov/WindowsAzure/issues/43) - Mapping for enum property [enhancement, serializer]

# 1.0.3 (22 September 2016)
- [#44](https://github.com/dtretyakov/WindowsAzure/pull/44) - Batches not always getting executed contributed by [richiej84](https://github.com/richiej84)
- [#50](https://github.com/dtretyakov/WindowsAzure/pull/50) - Fixes dtretyakov/WindowsAzure#35 contributed by [MarkDeVerno](https://github.com/MarkDeVerno)
- [#28](https://github.com/dtretyakov/WindowsAzure/issues/28) - TableSet.AddOrUpdate(IEnumerable) does not throw an exception on error [bug]
- [#35](https://github.com/dtretyakov/WindowsAzure/issues/35) - Bad request with Linq Query [bug, expressions]

# 1.0.1 (11 March 2015)
- [#42](https://github.com/dtretyakov/WindowsAzure/pull/42) - Methods to create table on tableset contributed by [tdietrich513](https://github.com/tdietrich513)
- [#18](https://github.com/dtretyakov/WindowsAzure/issues/18) - Add Fluent mappings for POCO [enhancement]
- [#40](https://github.com/dtretyakov/WindowsAzure/issues/40) - Latest version isn't on NuGet
- [#41](https://github.com/dtretyakov/WindowsAzure/issues/41) - No happy-path for creating a table if it doesn't exist. [enhancement]

# 1.0.0 (16 February 2015)
- [#30](https://github.com/dtretyakov/WindowsAzure/pull/30) - added InsertOrMerge Table Operation contributed by [sbetzin](https://github.com/sbetzin)
- [#34](https://github.com/dtretyakov/WindowsAzure/pull/34) - Fluent mapping and scripts to build &amp; publish to nuget contributed by [gblmarquez](https://github.com/gblmarquez)
- [#37](https://github.com/dtretyakov/WindowsAzure/pull/37) - updated nuget packages: unit tests green contributed by [rleopold](https://github.com/rleopold)
- [#29](https://github.com/dtretyakov/WindowsAzure/issues/29) - TableSet.Add(entities) doesn't work properly. [bug]

# 0.7.7 (10 August 2013)
- [#23](https://github.com/dtretyakov/WindowsAzure/issues/23) - Exception on cast enum to integer in the query expression [bug, expressions]
- [#24](https://github.com/dtretyakov/WindowsAzure/issues/24) - TakeAsync should return Task&lt;List&gt; [bug, extensions]
- [#25](https://github.com/dtretyakov/WindowsAzure/issues/25) - Unable to receive result in some composite queries [bug, expressions]
- [#26](https://github.com/dtretyakov/WindowsAzure/issues/26) - No GetBlobReferenceFromServerAsync method [enhancement, extensions]

# 0.7.6 (19 May 2013)
- [#20](https://github.com/dtretyakov/WindowsAzure/issues/20) - Exception &quot;Invalid entity member type:&quot; is raised for properties tagged with [Ignore] [bug]
- [#22](https://github.com/dtretyakov/WindowsAzure/issues/22) - System.ArgumentException: Member 'Convert' does not supported [bug]

# 0.7.5 (16 May 2013)
- [#21](https://github.com/dtretyakov/WindowsAzure/issues/21) - .FirstOrDefault(expression) invalid behaviour [bug]

# 0.7.4 (10 May 2013)
- [#16](https://github.com/dtretyakov/WindowsAzure/issues/16) - Add ability to use LINQ Contains method [enhancement]
- [#19](https://github.com/dtretyakov/WindowsAzure/issues/19) - Exception was thrown when Where expression terminated by First / FirstOrDefault [bug]

# 0.7.3 (27 April 2013)
- [#15](https://github.com/dtretyakov/WindowsAzure/issues/15) - Failed to execute LINQ queries from Web API OData [bug]
- [#17](https://github.com/dtretyakov/WindowsAzure/issues/17) - Return IEnumerable&lt;T&gt; in range method overloads [bug, enhancement]

# 0.7.2 (22 April 2013)
- [#3](https://github.com/dtretyakov/WindowsAzure/issues/3) - Add ability to use LINQ projections in queries [enhancement]
- [#5](https://github.com/dtretyakov/WindowsAzure/issues/5) - Add ability to use synchronous LINQ methods like Single / First [enhancement]
- [#6](https://github.com/dtretyakov/WindowsAzure/issues/6) - Add ability to group collection entities in TableSet by partition key [enhancement]
- [#11](https://github.com/dtretyakov/WindowsAzure/issues/11) - Create symbol package [bug, enhancement]
- [#12](https://github.com/dtretyakov/WindowsAzure/issues/12) - Add ToListAsync overload with a predicate [enhancement]
- [#13](https://github.com/dtretyakov/WindowsAzure/issues/13) - Unable to execute LINQ query with Invocation expression [bug]

# 0.7.0 (12 April 2013)
- [#1](https://github.com/dtretyakov/WindowsAzure/issues/1) - Possibly should rename method [bug]
- [#2](https://github.com/dtretyakov/WindowsAzure/issues/2) - Make LINQ query translator thread safe [bug]
- [#4](https://github.com/dtretyakov/WindowsAzure/issues/4) - Add async blob extensions [enhancement]
- [#8](https://github.com/dtretyakov/WindowsAzure/issues/8) - Got exception when passing a lambda expression with an enum-type comparison to SingleOrDefaultAsync[T](IQueryable`1 source, Expression`1 predicate, CancellationToken cancellationToken) method [bug]
- [#10](https://github.com/dtretyakov/WindowsAzure/issues/10) - CloudPageBlob CreateAsync extension method does not exist [bug]

