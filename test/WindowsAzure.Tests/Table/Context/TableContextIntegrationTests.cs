using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsAzure.Table;
using WindowsAzure.Tests.Attributes;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;

namespace WindowsAzure.Tests.Table.Context
{
    public sealed class TableContextIntegrationTests : CountryTableSetBase
    {
        [IntegrationFact]
        public void CreateUpdatedAndDeleteEntities()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            IList<Country> countries = ObjectsFactory.GetCountries();

            // Act
            List<Country> addedEntities = tableSet.Add(countries).ToList();

            foreach (Country country in addedEntities)
            {
                country.Population *= 2;
                country.Area *= 1.3;
            }

            List<Country> updatedEntities = tableSet.Update(addedEntities).ToList();

            tableSet.Remove(updatedEntities);
        }

        [IntegrationFact]
        public async Task CreateUpdatedAndDeleteEntitiesAsync()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            IList<Country> countries = ObjectsFactory.GetCountries();

            // Act
            List<Country> addedEntities = (await tableSet.AddAsync(countries)).ToList();

            foreach (Country country in addedEntities)
            {
                country.Population *= 2;
                country.Area *= 1.3;
            }

            List<Country> updatedEntities = (await tableSet.UpdateAsync(addedEntities)).ToList();

            await tableSet.RemoveAsync(updatedEntities);
        }

        [IntegrationFact]
        public void DeleteEntityByCompositeKey()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            Country country = ObjectsFactory.GetCountry();

            // Act
            Country addedEntity = tableSet.Add(country);

            tableSet.Remove(
                new Country
                    {
                        Continent = addedEntity.Continent,
                        Name = addedEntity.Name
                    });
        }

        [IntegrationFact]
        public async Task DeleteEntityByCompositeKeyAsync()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            Country country = ObjectsFactory.GetCountry();

            // Act
            Country addedEntity = await tableSet.AddAsync(country);

            await tableSet.RemoveAsync(
                new Country
                    {
                        Continent = addedEntity.Continent,
                        Name = addedEntity.Name
                    });
        }
    }
}