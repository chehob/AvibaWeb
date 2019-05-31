using System;
using System.Linq;
using System.Net.Http;
using AvibaWeb.DomainModels;
using AvibaWeb.Hubs;
using AvibaWeb.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Remotion.Linq.Clauses;
using TableDependency.Enums;
using TableDependency.EventArgs;
using TableDependency.SqlClient;

namespace AvibaWeb.Data
{
    public interface IDatabaseSubscription
    {
        void Configure(string connectionString);
    }

    public class CollectionOperationDatabaseSubscription : IDatabaseSubscription
    {
        private bool disposedValue = false;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<MessageHub> _hubContext;
        private SqlTableDependency<CollectionOperation> _tableDependency;
        private readonly IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;

        public CollectionOperationDatabaseSubscription(IServiceScopeFactory scopeFactory,
            IHubContext<MessageHub> hubContext, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public void Configure(string connectionString)
        {
            _tableDependency = new SqlTableDependency<CollectionOperation>(
                connectionString, "CollectionOperations", null, null, null, null, DmlTriggerType.Insert);
            _tableDependency.OnChanged += Changed;
            _tableDependency.OnError += TableDependency_OnError;
            _tableDependency.Start();

            Console.WriteLine("Waiting for receiving notifications...");
        }

        private void TableDependency_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"SqlTableDependency error: {e.Error.Message}");
        }

        private async void Changed(object sender, RecordChangedEventArgs<CollectionOperation> e)
        {
            if (e.ChangeType == ChangeType.None) return;
            
            var changedEntity = e.Entity;
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
                var result = (from collector in dbContext.Users
                    join collection in dbContext.Collections on collector.Id equals collection.CollectorId
                    join provider in dbContext.Users on collection.ProviderId equals provider.Id
                    join deskIssued in dbContext.Desks on collection.DeskIssuedId equals deskIssued.DeskId into desks
                    from desk in desks.DefaultIfEmpty()
                    where collection.CollectionId == changedEntity.CollectionId
                    select new
                    {
                        CollectorId = collector.Id,
                        CollectorPushAllId = collector.PushAllUserId,
                        CollectionAmount = collection.Amount,
                        CollectionDesk = desk.Description,
                        ProviderName = provider.Name
                    }).FirstOrDefault();

                if (result == null) return;

                const string topic = "AMS";
                var message = $"Поступила новая инкассация {result.ProviderName} {result.CollectionDesk} {result.CollectionAmount:### ### ##0.00}";
                await _hubContext.Clients.User(result.CollectorId).SendAsync("ReceiveMessage", message, topic);

                if (result.CollectorPushAllId == null) return;
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://pushall.ru/");
                var response = await client.PostAsync("api.php", new MultipartFormDataContent
                {
                    {new StringContent("unicast"), "type"},
                    {new StringContent(_configuration["PushAll:ChannelId"]), "id"},
                    {new StringContent(_configuration["PushAll:ApiKey"]), "key"},
                    {new StringContent(message), "text"},
                    {new StringContent("AMS"), "title"},
                    {new StringContent("300"), "ttl"},
                    {new StringContent(result.CollectorPushAllId.ToString()), "uid"}
                });
            }
        }

        #region IDisposable

        ~CollectionOperationDatabaseSubscription()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _tableDependency.Stop();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
