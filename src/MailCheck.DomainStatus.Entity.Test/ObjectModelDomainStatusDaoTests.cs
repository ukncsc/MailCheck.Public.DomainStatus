using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using FakeItEasy;
using MailCheck.DomainStatus.Common.Dao;
using MailCheck.DomainStatus.Contracts;
using MailCheck.DomainStatus.Entity.Dao;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.DomainStatus.Entity.Test
{
    [TestFixture]
    public class ObjectModelDomainStatusDaoTests
    {
        private IDynamoDBContext _dynamoDbContext;
        private ILogger<ObjectModelDomainStatusDao> _logger;
        private ObjectModelDomainStatusDao _objectModelDomainStatusDao;

        [SetUp]
        public void SetUp()
        {
            _dynamoDbContext = A.Fake<IDynamoDBContext>();
            _logger = A.Fake<ILogger<ObjectModelDomainStatusDao>>();
            _objectModelDomainStatusDao = new ObjectModelDomainStatusDao(_dynamoDbContext, _logger);
        }

        [Test]
        public void RetriesSaveThreeTimesOnConditionalCheckFail()
        {
            A.CallTo(() => _dynamoDbContext.LoadAsync< DomainStatusEntity>(A<string>._, A<DynamoDBOperationConfig>._, A<CancellationToken>._))
                .Returns(new DomainStatusEntity("", new Dictionary<string, Common.Dao.Record>()));

            A.CallTo(() => _dynamoDbContext.SaveAsync(A<DomainStatusEntity>._, A<DynamoDBOperationConfig>._, A<CancellationToken>._))
                .Throws(() => new ConditionalCheckFailedException(""));

            Assert.ThrowsAsync<ConditionalCheckFailedException>(() => _objectModelDomainStatusDao.Save("testDomain", "SPF", Status.Error));

            A.CallTo(() => _dynamoDbContext.SaveAsync(A<DomainStatusEntity>._, A<DynamoDBOperationConfig>._, A<CancellationToken>._))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 3);
        }

        [Test]
        public void ThrowsImmediatelyOnNonConditionalCheckExceptions()
        {
            A.CallTo(() => _dynamoDbContext.LoadAsync<DomainStatusEntity>(A<string>._, A<DynamoDBOperationConfig>._, A<CancellationToken>._))
                .Throws(new Exception("testExceptionMessage"));

            Exception exception = Assert.ThrowsAsync<Exception>(() => _objectModelDomainStatusDao.Save("testDomain", "SPF", Status.Error));

            Assert.AreEqual("testExceptionMessage", exception.Message);

            A.CallTo(() => _dynamoDbContext.SaveAsync(A<DomainStatusEntity>._, A<DynamoDBOperationConfig>._, A<CancellationToken>._))
                .MustNotHaveHappened();
        }
    }
}
