using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using MailCheck.Common.Data.Abstractions;
using MailCheck.Common.Data.Implementations;
using MailCheck.Common.Environment.Abstractions;
using MailCheck.Common.Environment.Implementations;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.SSM;
using MailCheck.DomainStatus.Contracts;
using MailCheck.DomainStatus.Entity.Dao;
using MailCheck.DomainStatus.Entity.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace MailCheck.DomainStatus.Entity.StartUp
{
    internal class StartUp : IStartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>()
                .AddSingleton<IDynamoDBContext, DynamoDBContext>()
                .AddTransient<IConnectionInfoAsync, MySqlEnvironmentParameterStoreConnectionInfoAsync>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddSingleton<IAmazonSimpleSystemsManagement, CachingAmazonSimpleSystemsManagementClient>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>()
                .AddTransient<IHandle<DomainStatusEvaluation>, DomainStatusEvaluationHandler>()
                .AddTransient<IDomainStatusDao, ObjectModelDomainStatusDao>();
        }
    }
}