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
    public sealed class WhereTests
    {
        private const string Germany = "Germany";
        private const string Europe = "Europe";
        private const string Id = "829ea8b2-3bd5-45a4-8b54-533c69e608d7";

        private readonly IQueryable<Country> _countries;
        private readonly Dictionary<string, string> _nameChanges;

        public WhereTests()
        {
            _countries = new EnumerableQuery<Country>(new Country[] {});
            _nameChanges = new Dictionary<string, string>
                {
                    {"Continent", "PartitionKey"},
                    {"Name", "RowKey"}
                };
        }

        [Fact]
        public void UseWhereOnPartitionKey()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Continent == Europe);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("PartitionKey eq 'Europe'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnRowKey()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Name == Germany);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey eq 'Germany'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseEnumValueInWhereOnRowKey()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Name == Countries.Germany.ToString());
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey eq 'Germany'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnDouble()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Area < 350000);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Area lt 350000.0", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnBytes()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.TopSecretKey == new byte[] {0xff, 0xee, 0xdd});
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("TopSecretKey eq X'ffeedd'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnDateTime()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Formed < new DateTime(1800, 1, 1));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Formed lt datetime'1800-01-01T00:00:00'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnGuid()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Id == new Guid(Id));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Id eq guid'829ea8b2-3bd5-45a4-8b54-533c69e608d7'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnBoolean()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.IsExists);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("IsExists", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnInt64()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Population >= 80000000L);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Population ge 80000000L", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UseWhereOnInt32()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.PresidentsCount <= 10);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("PresidentsCount le 10", translation.TableQuery.FilterString);
        }

        [Fact]
        public void BinaryQueryWithEqualBooleanValue()
        {
            // Arrange
            const bool value = false;
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.IsExists == value);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("IsExists eq false", translation.TableQuery.FilterString);
        }

        [Fact]
        public void BinaryQueryWithNotEqualBooleanValue()
        {
            // Arrange
            const bool value = false;
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.IsExists != value);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("IsExists ne false", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UnaryQueryWithBooleanValue()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.IsExists);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("IsExists", translation.TableQuery.FilterString);
        }

        [Fact]
        public void UnaryQueryWithInversedBooleanValue()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => !p.IsExists);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("not IsExists", translation.TableQuery.FilterString);
        }

        // ReSharper disable CompareOfFloatsByEqualityOperator

        [Fact]
        public void BinaryExpressionWithExternalMethodCall()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Area == Convert.ToDouble(555));
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("Area eq 555.0", translation.TableQuery.FilterString);
        }

        // ReSharper restore CompareOfFloatsByEqualityOperator

        [Fact]
        public void BinaryExpressionWithComplexExpression()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => Math.Abs(p.Area - Convert.ToDouble(555)) < 0.2);
            var translation = new TranslationResult();

            // Act
            Assert.Throws<NotSupportedException>(() => translator.Translate((MethodCallExpression) query.Body, translation));

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Null(translation.TableQuery.FilterString);
        }

        [Fact]
        public void BinaryExpressionWithPrivateMethodCall()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Name == GetName());
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("RowKey eq 'new name'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void WhereWithInvalidMethod()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            Expression<Func<IQueryable<Country>>> query = () => _countries.Take(2);
            var translation = new TranslationResult();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() => translator.Translate((MethodCallExpression) query.Body, translation));

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Null(translation.TableQuery.FilterString);
        }

        [Fact]
        public void WhereWithNullableDateTime()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            var queryable = new EnumerableQuery<EntityWithFields>(new List<EntityWithFields>());
            var value = new DateTime(1980, 1, 1);
            Expression<Func<IQueryable<EntityWithFields>>> query = () => queryable.Where(p => p.NullableDateTime < value);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Equal("NullableDateTime lt datetime'1980-01-01T00:00:00'", translation.TableQuery.FilterString);
        }

        [Fact]
        public void WhereWithNullableInt32()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            var queryable = new EnumerableQuery<EntityWithFields>(new List<EntityWithFields>());
            const int value = 33;
            Expression<Func<IQueryable<EntityWithFields>>> query = () => queryable.Where(p => p.NullableInt32 <= value);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Equal("NullableInt32 le 33", translation.TableQuery.FilterString);
        }

        [Fact]
        public void WhereWithNullableInt64()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            var queryable = new EnumerableQuery<EntityWithFields>(new List<EntityWithFields>());
            const long value = 22L;
            Expression<Func<IQueryable<EntityWithFields>>> query = () => queryable.Where(p => p.NullableInt64 >= value);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Equal("NullableInt64 ge 22L", translation.TableQuery.FilterString);
        }

        [Fact] 
        public void WhereWithNullableDouble()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            var queryable = new EnumerableQuery<EntityWithFields>(new List<EntityWithFields>());
            const double value = .3;
            Expression<Func<IQueryable<EntityWithFields>>> query = () => queryable.Where(p => p.NullableDouble > value);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Equal("NullableDouble gt .3", translation.TableQuery.FilterString);
        }

        [Fact]
        public void WhereWithOverloadedToString()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            var queryable = new EnumerableQuery<EntityWithFields>(new List<EntityWithFields>());
            var value = new EntityWithToString("My value");
            Expression<Func<IQueryable<EntityWithFields>>> query = () => queryable.Where(p => p.String == value.ToString());
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Equal(string.Format("String eq '{0}'", value), translation.TableQuery.FilterString);
        }

        // ReSharper disable ConvertToConstant.Local

        [Fact]
        public void WhereWithEnumartionValue()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            var queryable = new EnumerableQuery<EntityWithFields>(new List<EntityWithFields>());

            var value = Countries.Germany;

            Expression<Func<IQueryable<EntityWithFields>>> query = () => queryable.Where(p => p.String == value.ToString());
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Equal(string.Format("String eq '{0}'", value), translation.TableQuery.FilterString);
        }

        // ReSharper restore ConvertToConstant.Local

        [Fact]
        public void WhereWithEnumartionConstantValue()
        {
            // Arrange
            var translator = new WhereTranslator(_nameChanges);
            var queryable = new EnumerableQuery<EntityWithFields>(new List<EntityWithFields>());

            const Countries value = Countries.Germany;

            Expression<Func<IQueryable<EntityWithFields>>> query = () => queryable.Where(p => p.String == value.ToString());
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression)query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Equal(string.Format("String eq '{0}'", value), translation.TableQuery.FilterString);
        }

        private string GetName()
        {
            return "new name";
        }
    }
}