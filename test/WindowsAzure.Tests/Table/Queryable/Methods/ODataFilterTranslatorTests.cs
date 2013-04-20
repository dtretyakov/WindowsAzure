using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.Expressions;
using WindowsAzure.Table.Queryable.Expressions.Methods;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Methods
{
    public sealed class ODataFilterTranslatorTests
    {
        private const string Germany = "Germany";
        private const string Spain = "Spain";
        private const string Europe = "Europe";
        private const string Id = "829ea8b2-3bd5-45a4-8b54-533c69e608d7";

        private readonly IQueryable<Country> _countries;
        private readonly Dictionary<string, string> _nameChanges;

        public ODataFilterTranslatorTests()
        {
            _countries = new EnumerableQuery<Country>(new Country[] {});
            _nameChanges = new Dictionary<string, string>
                {
                    {"Continent", "PartitionKey"},
                    {"Name", "RowKey"}
                };
        }

        private static IEnumerable<Country> GetList()
        {
            return new List<Country>
                {
                    new Country
                        {
                            Area = 357021,
                            Continent = "Europe",
                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                            Formed = new DateTime(1871, 1, 18),
                            Id = Guid.NewGuid(),
                            IsExists = true,
                            Name = Germany,
                            Population = 81799600,
                            PresidentsCount = 11
                        },
                    new Country
                        {
                            Area = 505992,
                            Continent = "Europe",
                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                            Formed = new DateTime(1812, 1, 1),
                            Id = Guid.NewGuid(),
                            IsExists = false,
                            Name = Spain,
                            Population = 47190493,
                            PresidentsCount = 8
                        }
                };
        }

        [Fact]
        public void UseWhereOnPartitionKeyTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Continent == Europe);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("PartitionKey eq 'Europe'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Name == Germany);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey eq 'Germany'", translation.TableQuery.FilterString);
        }

        // ReSharper disable StringCompareToIsCultureSpecific

        [Fact]
        public void UseCompareToInWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Name.CompareTo("F") >= 0 && p.Name.CompareTo("G") <= 0);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey ge 'F' and RowKey le 'G'", translation.TableQuery.FilterString);
        }

        // ReSharper restore StringCompareToIsCultureSpecific

        [Fact]
        public void UseCompareInWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query =
                () => _countries.Where(p => String.Compare(p.Name, "F", StringComparison.Ordinal) >= 0 && String.Compare(p.Name, "G", StringComparison.Ordinal) <= 0);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey ge 'F' and RowKey le 'G'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseCompareOrdinalInWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => String.CompareOrdinal(p.Name, "F") >= 0 && String.CompareOrdinal(p.Name, "G") <= 0);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey ge 'F' and RowKey le 'G'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseEnumValueInWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Name == Countries.Germany.ToString());
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey eq 'Germany'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnDoubleTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Area < 350000);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Area lt 350000.0", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnBytesTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.TopSecretKey == new byte[] {0xff, 0xee, 0xdd});
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("TopSecretKey eq X'ffeedd'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnDateTimeTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Formed < new DateTime(1800, 1, 1));
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Formed lt datetime'1800-01-01T00:00:00'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnGuidTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Id == new Guid(Id));
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Id eq guid'829ea8b2-3bd5-45a4-8b54-533c69e608d7'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnBooleanTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.IsExists);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("IsExists", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnInt64Test()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Population >= 80000000L);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Population ge 80000000L", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnInt32Test()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.PresidentsCount <= 10);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("PresidentsCount le 10", translation.TableQuery.FilterString);
        }

        [Fact]
        public void BinaryQueryWithEqualBooleanValueTest()
        {
            // Arrange
            const bool value = false;
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.IsExists == value);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("IsExists eq false", translation.TableQuery.FilterString);
        }

        [Fact]
        public void BinaryQueryWithNotEqualBooleanValueTest()
        {
            // Arrange
            const bool value = false;
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.IsExists != value);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("IsExists ne false", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UnaryQueryWithBooleanValueTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.IsExists);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("IsExists", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UnaryQueryWithInversedBooleanValueTest()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => !p.IsExists);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("not IsExists", translation.TableQuery.FilterString);
        }

        [Fact]
        public void LinqFirstClause()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<Country>> query = () => _countries.First(p => !p.IsExists);
            var translation = new TranslationResult();

            // Act && Assert
            translator.Translate(translation, (MethodCallExpression) query.Body);

            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("not IsExists", translation.TableQuery.FilterString);
        }

        [Fact]
        public void LinqFirstOrDefaultClause()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<Country>> query = () => _countries.FirstOrDefault(p => !p.IsExists);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("not IsExists", translation.TableQuery.FilterString);
        }

        [Fact]
        public void LinqSingleClause()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<Country>> query = () => _countries.Single(p => !p.IsExists);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("not IsExists", translation.TableQuery.FilterString);
        }

        [Fact]
        public void LinqSingleOrDefaultClause()
        {
            // Arrange
            var translator = new ODataFilterTranslator(_nameChanges);
            Expression<Func<Country>> query = () => _countries.SingleOrDefault(p => !p.IsExists);
            var translation = new TranslationResult();

            // Act
            translator.Translate(translation, (MethodCallExpression) query.Body);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("not IsExists", translation.TableQuery.FilterString);
        }
    }
}