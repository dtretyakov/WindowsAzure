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
    public sealed class StringCompareTests
    {
        private readonly IQueryable<Country> _countries;
        private readonly Dictionary<string, string> _nameChanges;

        public StringCompareTests()
        {
            _countries = new EnumerableQuery<Country>(new Country[] {});
            _nameChanges = new Dictionary<string, string>
                {
                    {"Continent", "PartitionKey"},
                    {"Name", "RowKey"}
                };
        }

        // ReSharper disable StringCompareToIsCultureSpecific

        [Fact]
        public void UseCompareToInWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Name.CompareTo("F") >= 0 && p.Name.CompareTo("G") <= 0);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

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
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query =
                () => _countries.Where(p => String.Compare(p.Name, "F", StringComparison.Ordinal) >= 0 && String.Compare(p.Name, "G", StringComparison.Ordinal) <= 0);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey ge 'F' and RowKey le 'G'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseCompareOrdinalInWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => String.CompareOrdinal(p.Name, "F") >= 0 && String.CompareOrdinal(p.Name, "G") <= 0);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey ge 'F' and RowKey le 'G'", translation.TableQuery.FilterString);
        }
    }
}