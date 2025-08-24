using System.Data;

namespace Server.Services
{
    public class WorkerServices : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<WorkerServices> _logger;

        public WorkerServices(IServiceProvider services, ILogger<WorkerServices> logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FunewsManagementContext>();
            var conn = db.Database.GetDbConnection();

            try
            {
                await conn.OpenAsync(stoppingToken);

                using var checkCmd = conn.CreateCommand();
                checkCmd.CommandText = @"
SELECT COUNT(*) 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'RefreshTokens';";
                checkCmd.CommandType = CommandType.Text;

                var result = await checkCmd.ExecuteScalarAsync(stoppingToken);
                var exists = Convert.ToInt32(result) > 0;

                if (!exists)
                {
                    _logger.LogInformation("RefreshTokens table not found. Creating table (with FK to SystemAccount) ...");

                    using var createCmd = conn.CreateCommand();
                    createCmd.CommandText = @"
CREATE TABLE [dbo].[RefreshTokens](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [AccountId] SMALLINT NOT NULL,
    [AccessTokenHash] NVARCHAR(512) NOT NULL,
    [RefreshTokenHash] NVARCHAR(512) NOT NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    CONSTRAINT FK_RefreshTokens_Account FOREIGN KEY (AccountId) REFERENCES [dbo].[SystemAccount](AccountID)
); 
CREATE INDEX IX_RefreshTokens_AccountId ON [dbo].[RefreshTokens](AccountId);";
                    createCmd.CommandType = CommandType.Text;

                    await createCmd.ExecuteNonQueryAsync(stoppingToken);
                    _logger.LogInformation("RefreshTokens table created successfully with FK to SystemAccount.");
                }
                else
                {
                    _logger.LogInformation("RefreshTokens table already exists.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when checking/creating RefreshTokens table.");
            }
            finally
            {
                try { await conn.CloseAsync(); } catch { /* ignore */ }
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
