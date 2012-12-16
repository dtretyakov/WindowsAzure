﻿using System;
using System.Collections.Generic;
using System.Linq;
using WindowsAzure.Table;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Context.Query
{
    public sealed class WhereWithDifferentTypesTests : TableSetTestBase
    {
        private const string Germany = "Germany";
        private const string Spain = "Spain";
        private const string Finland = "Finland";
        private const string France = "France";
        private const string Europe = "Europe";

        private const string Id = "829ea8b2-3bd5-45a4-8b54-533c69e608d7";

        public WhereWithDifferentTypesTests()
        {
            TableSet<Country> tableSet = GetTableSet();
            tableSet.Add(
                new List<Country>
                    {
                        new Country
                            {
                                Area = 357021,
                                Continent = Europe,
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
                                Continent = Europe,
                                TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                Formed = new DateTime(1812, 1, 1),
                                Id = new Guid(Id),
                                IsExists = false,
                                Name = Spain,
                                Population = 47190493,
                                PresidentsCount = 8
                            },
                        new Country
                            {
                                Area = 674843,
                                Continent = Europe,
                                TopSecretKey = new byte[] {0xff, 0xee, 0xdd},
                                Formed = new DateTime(1792, 1, 1),
                                Id = Guid.NewGuid(),
                                IsExists = true,
                                Name = France,
                                Population = 65350000,
                                PresidentsCount = 24
                            },
                        new Country
                            {
                                Area = 338424,
                                Continent = Europe,
                                TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                Formed = new DateTime(1809, 3, 29),
                                Id = Guid.NewGuid(),
                                IsExists = true,
                                Name = Finland,
                                Population = 5421827,
                                PresidentsCount = 12
                            }
                    });
        }

        [Fact]
        public void UseWhereOnPartitionKeyTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            List<Country> entities = tableSet.Where(p => p.Continent == Europe).ToList();

            // Assert
            Assert.NotNull(entities);
            Assert.Equal(entities.Count, 4);
            Assert.Equal(entities[0].Continent, Europe);
        }

        [Fact]
        public void UseWhereOnRowKeyTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            List<Country> entities = tableSet.Where(p => p.Name == Germany).ToList();

            // Assert
            Assert.NotNull(entities);
            Assert.Equal(entities.Count, 1);
            Assert.Equal(entities[0].Name, Germany);
        }

        [Fact]
        public void UseWhereOnDoubleTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            List<Country> entities = tableSet.Where(p => p.Area < 350000).ToList();

            // Assert
            Assert.NotNull(entities);
            Assert.Equal(entities.Count, 1);
            Assert.Equal(entities[0].Name, Finland);
        }

        [Fact]
        public void UseWhereOnBytesTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            List<Country> entities = tableSet.Where(p => p.TopSecretKey == new byte[] {0xff, 0xee, 0xdd}).ToList();

            // Assert
            Assert.NotNull(entities);
            Assert.Equal(entities.Count, 1);
            Assert.Equal(entities[0].Name, France);
        }

        [Fact]
        public void UseWhereOnDateTimeTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            List<Country> entities = tableSet.Where(p => p.Formed < new DateTime(1800, 1, 1)).ToList();

            // Assert
            Assert.NotNull(entities);
            Assert.Equal(entities.Count, 1);
            Assert.Equal(entities[0].Name, France);
        }

        [Fact]
        public void UseWhereOnGuidTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            List<Country> entities = tableSet.Where(p => p.Id == new Guid(Id)).ToList();

            // Assert
            Assert.NotNull(entities);
            Assert.Equal(entities.Count, 1);
            Assert.Equal(entities[0].Name, Spain);
        }

        [Fact]
        public void UseWhereOnBooleanTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            List<Country> entities = tableSet.Where(p => p.IsExists).ToList();

            // Assert
            Assert.NotNull(entities);
            Assert.Equal(entities.Count, 3);
        }

        [Fact]
        public void UseWhereOnInt64Test()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            List<Country> entities = tableSet.Where(p => p.Population >= 80000000L).ToList();

            // Assert
            Assert.NotNull(entities);
            Assert.Equal(entities.Count, 1);
            Assert.Equal(entities[0].Name, Germany);
        }

        [Fact]
        public void UseWhereOnInt32Test()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            List<Country> entities = tableSet.Where(p => p.PresidentsCount <= 10).ToList();

            // Assert
            Assert.NotNull(entities);
            Assert.Equal(entities.Count, 1);
            Assert.Equal(entities[0].Name, Spain);
        }
    }
}