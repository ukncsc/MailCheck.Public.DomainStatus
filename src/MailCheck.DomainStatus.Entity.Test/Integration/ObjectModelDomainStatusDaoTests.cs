using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using FakeItEasy;
using MailCheck.DomainStatus.Common.Dao;
using MailCheck.DomainStatus.Contracts;
using MailCheck.DomainStatus.Entity.Dao;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.DomainStatus.Entity.Test.Integration
{
    [TestFixture(Category = "Integration")]
    public class ObjectModelDomainStatusDaoTests
    {
        private static readonly string TestTableName = $"DomainStatusTestTable-{Guid.NewGuid().ToString("N")}";
        private static readonly DynamoDBOperationConfig OperationConfig = new DynamoDBOperationConfig 
        {
            OverrideTableName = TestTableName
        };

        private readonly AmazonDynamoDBClient Client = new AmazonDynamoDBClient();
        private ILogger<ObjectModelDomainStatusDao> _logger;
        private DynamoDBContext _context;
        private ObjectModelDomainStatusDao _dao;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var createTableRequest = new CreateTableRequest
            {
                TableName = TestTableName,
                AttributeDefinitions = 
                {
                    new AttributeDefinition("Domain", ScalarAttributeType.S),
                },
                KeySchema = 
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Domain",
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 6
                },
            };

            var createTableResponse = await Client.CreateTableAsync(createTableRequest);

            if (createTableResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Create table request sent for: {TestTableName}");
            }
            else
            {
                throw new AssertionException($"Failed to created table {TestTableName} with status code {createTableResponse.HttpStatusCode}");
            }

            await Retry(
                3,
                TimeSpan.FromSeconds(5),
                $"Wait for table {TestTableName} to be ACTIVE",
                async () => (await Client.DescribeTableAsync(TestTableName)).Table.TableStatus == "ACTIVE"
            );

            Console.WriteLine($"Created table successfully and is now active: {TestTableName}");
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            try
            {
                var deleteTableResponse = await Client.DeleteTableAsync(TestTableName);

                if (deleteTableResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine($"Delete table request accepted for table: {TestTableName}");
                }
                else
                {
                    throw new AssertionException($"Could not delete Dynamo table used in test: {TestTableName}, failed with status code {deleteTableResponse.HttpStatusCode}");
                }
            }
            catch (ResourceNotFoundException)
            {
                Console.WriteLine($"Attempted to delete table {TestTableName} but was not found");
                return;
            }

            await Retry(
                3,
                TimeSpan.FromSeconds(5),
                "Waiting for table to be removed",
                async () => (await Client.ListTablesAsync()).TableNames.Contains(TestTableName)
            );
        }

        [SetUp]
        public void SetUp()
        {
            _logger = A.Fake<ILogger<ObjectModelDomainStatusDao>>();
            _context = new DynamoDBContext(Client);
            _dao = new ObjectModelDomainStatusDao(_context, _logger)
            {
                Config = OperationConfig
            };
        }

        [Test]
        public async Task TestPersistingDomainStatus()
        {
            await _dao.Save("test-domain.com", "SPF", Status.Error);

            var getItemRequest = new GetItemRequest
            {
                TableName = TestTableName,
                Key = 
                {
                    ["Domain"] = new AttributeValue { S = "test-domain.com" }
                }
            };

            var resultFromDb = await Client.GetItemAsync(getItemRequest);

            Assert.That(resultFromDb, Is.Not.Null);
        }

        [Test]
        public async Task TestOverwritingDomainStatus()
        {
            await _dao.Save("test-domain.com", "SPF", Status.Error);
            await _dao.Save("test-domain.com", "TLS", Status.Warning);
            await _dao.Save("test-domain.com", "SPF", Status.Warning);

            var entity = await _context.LoadAsync<DomainStatusEntity>("test-domain.com", OperationConfig);

            Assert.AreEqual("test-domain.com", entity.Domain);
            Assert.AreEqual(2, entity.Version);
            Assert.AreEqual(1, entity.SchemaVersion);
            Assert.AreEqual(2, entity.Records.Count);

            Assert.IsTrue(entity.Records.ContainsKey("SPF"));
            Assert.AreEqual("SPF", entity.Records["SPF"].RecordType);
            Assert.AreEqual(Status.Warning, entity.Records["SPF"].Status);

            Assert.IsTrue(entity.Records.ContainsKey("TLS"));
            Assert.AreEqual("TLS", entity.Records["TLS"].RecordType);
            Assert.AreEqual(Status.Warning, entity.Records["TLS"].Status);
        }

        static async Task Retry(int times, TimeSpan delay, string label, Func<Task<bool>> func)
        {
            bool outcome;
            while ((outcome = await func()) == false && times-- > 0)
            {
                await Task.Delay(delay);
            }

            if (!outcome)
            {
                throw new Exception($"Operation {label} still not successful after {times} retries.");
            }
        }
    }
}