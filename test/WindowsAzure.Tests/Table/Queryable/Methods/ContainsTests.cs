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
    public sealed class ContainsTests
    {
        private readonly IQueryable<Country> _countries;
        private readonly Dictionary<string, string> _nameChanges;

        public ContainsTests()
        {
            _countries = new EnumerableQuery<Country>(new Country[] {});
            _nameChanges = new Dictionary<string, string>
                {
                    {"Continent", "PartitionKey"},
                    {"Name", "RowKey"}
                };
        }

        [Fact]
        public void UseContainsInWhereOnRowKeyTest()
        {
            // Arrange
            var list = new List<String>
                {
                    "Latvia",
                    "Germany"
                };
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => list.Contains(p.Name));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey eq 'Latvia' or RowKey eq 'Germany'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseNotContainsInWhereOnRowKeyTest()
        {
            // Arrange
            var list = new List<String>
                {
                    "Latvia",
                    "Germany"
                };
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => !list.Contains(p.Name));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey ne 'Latvia' or RowKey ne 'Germany'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseContainsAndEqualityInWhereOnRowKeyTest()
        {
            // Arrange
            var list = new List<String>
                {
                    "Latvia",
                    "Germany"
                };
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => list.Contains(p.Name) && p.Continent == "Africa");
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("(RowKey eq 'Latvia' or RowKey eq 'Germany') and PartitionKey eq 'Africa'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseNotContainsAndEqualityInWhereOnRowKeyTest()
        {
            // Arrange
            var list = new List<String>
                {
                    "Latvia",
                    "Germany"
                };
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => !list.Contains(p.Name) && p.Continent == "Africa");
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("(RowKey ne 'Latvia' or RowKey ne 'Germany') and PartitionKey eq 'Africa'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseBooleanContainsInWhereOnRowKeyTest()
        {
            // Arrange
            var list = new List<bool>
                {
                    true,
                    false
                };
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => list.Contains(p.IsExists));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("IsExists eq true or IsExists eq false", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseInt32ContainsInWhereOnRowKeyTest()
        {
            // Arrange
            var list = new List<Int32>
                {
                    0,
                    1
                };
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => list.Contains(p.PresidentsCount));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("PresidentsCount eq 0 or PresidentsCount eq 1", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseInt64ContainsInWhereOnRowKeyTest()
        {
            // Arrange
            var list = new List<Int64>
                {
                    0,
                    1
                };
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => list.Contains(p.Population));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Population eq 0L or Population eq 1L", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseDoubleContainsInWhereOnRowKeyTest()
        {
            // Arrange
            var list = new List<double>
                {
                    111.11,
                    222.22
                };
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => list.Contains(p.Area));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Area eq 111.11 or Area eq 222.22", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseGuidContainsInWhereOnRowKeyTest()
        {
            // Arrange
            var list = new List<Guid>
                {
                    new Guid("48ed2917-f7d4-4383-aa29-4062f1296bbc"),
                    new Guid("8a024e77-4f06-49d9-9d46-9a0e59d74fcd")
                };
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => list.Contains(p.Id));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Id eq guid'48ed2917-f7d4-4383-aa29-4062f1296bbc' or Id eq guid'8a024e77-4f06-49d9-9d46-9a0e59d74fcd'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseDateTimeContainsInWhereOnRowKeyTest()
        {
            // Arrange
            var list = new List<DateTime>
                {
                    DateTime.FromBinary(555),
                    DateTime.FromBinary(666)
                };
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => list.Contains(p.Formed));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Formed eq datetime'0001-01-01T00:00:00.0000555' or Formed eq datetime'0001-01-01T00:00:00.0000666'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseByteContainsInWhereOnRowKeyTest()
        {
            // Arrange
            var list = new List<byte[]>
                {
                    new byte[]{0x11, 0x22, 0x33},
                    new byte[]{0x44, 0x55, 0x66}
                };
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => list.Contains(p.TopSecretKey));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("TopSecretKey eq X'112233' or TopSecretKey eq X'445566'", translation.TableQuery.FilterString);
        }
    }
}