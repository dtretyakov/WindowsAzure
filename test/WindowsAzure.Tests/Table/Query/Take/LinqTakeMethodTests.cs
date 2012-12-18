using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable;
using WindowsAzure.Table.Queryable.Expressions.Methods;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Query.Take
{
    public sealed class LinqTakeMethodTests
    {
        private IQueryable<Country> GetQueryable()
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
                                Name = "Germany",
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
                                Name = "Spain",
                                Population = 47190493,
                                PresidentsCount = 8
                            },
                        new Country
                            {
                                Area = 674843,
                                Continent = "Europe",
                                TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                Formed = new DateTime(1792, 1, 1),
                                Id = Guid.NewGuid(),
                                IsExists = true,
                                Name = "France",
                                Population = 65350000,
                                PresidentsCount = 24
                            },
                        new Country
                            {
                                Area = 338424,
                                Continent = "Europe",
                                TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                Formed = new DateTime(1809, 3, 29),
                                Id = Guid.NewGuid(),
                                IsExists = true,
                                Name = "Finland",
                                Population = 5421827,
                                PresidentsCount = 12
                            }
                    }.AsQueryable();
                
        }

        [Fact]
        public void LinqTakeOneEntityTest()
        {
            // Arrange
            const int count = 435435;
            var queryable = GetQueryable();
            var query = queryable.Take(count);
            var takeTranslator = new TakeTranslator();

            // Act
            var result = takeTranslator.Translate(
                (MethodCallExpression) query.Expression,
                new Dictionary<string, string>());

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(QuerySegment.Top));
            Assert.Equal(result[QuerySegment.Top], count.ToString(CultureInfo.InvariantCulture));
        }
    }
}