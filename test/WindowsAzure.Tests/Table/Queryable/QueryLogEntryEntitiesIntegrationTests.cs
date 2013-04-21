using System;
using System.Threading.Tasks;
using WindowsAzure.Table;
using WindowsAzure.Table.Extensions;
using WindowsAzure.Tests.Attributes;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable
{
    public sealed class QueryLogEntryEntitiesIntegrationTests : LogEntryTableSetBase
    {
        private const string MessageTemplate = "My message {0}";

        public QueryLogEntryEntitiesIntegrationTests()
        {
            TableSet<LogEntry> tableSet = GetTableSet();

            for (int i = 0; i < 3; i ++)
            {
                tableSet.Add(
                    new LogEntry
                        {
                            Id = Guid.NewGuid().ToString("N"),
                            Message = string.Format(MessageTemplate, i)
                        });
            }
        }

        [IntegrationalFact]
        public async Task QueryLogEntryAndUpdateTest()
        {
            // Arrange
            const string value = "New message";
            TableSet<LogEntry> tableSet = GetTableSet();

            // Act
            LogEntry result = await tableSet.FirstAsync();
            result.Message = value;
            LogEntry updatedResult = await tableSet.UpdateAsync(result);

            // Assert
            Assert.Equal(result.Message, updatedResult.Message);
            Assert.NotEqual(result.ETag, updatedResult.ETag);
        }

        [IntegrationalFact]
        public async Task QueryLogEntryTwiceAndCheckETagTest()
        {
            // Arrange
            TableSet<LogEntry> tableSet = GetTableSet();

            // Act
            LogEntry result1 = await tableSet.FirstAsync();
            LogEntry result2 = await tableSet.SingleAsync(p => p.Id == result1.Id);

            // Assert
            Assert.Equal(result1.Id, result2.Id);
            Assert.Equal(result1.ETag, result2.ETag);
        }

        [IntegrationalFact]
        public async Task QueryLogEntryByMessageValueTest()
        {
            // Arrange
            TableSet<LogEntry> tableSet = GetTableSet();
            string message = string.Format(MessageTemplate, 2);

            // Act
            LogEntry result = await tableSet.FirstAsync(p => p.Message == message);

            // Assert
            Assert.Equal(result.Message, message);
        }
    }
}