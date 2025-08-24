using Entity.ModelResponse;

namespace Repo.Repository;

public interface ISystemAccountRepository : IGenericRepository<SystemAccount, SystemAccountResponse, short>
{
    Task<SystemAccount?> GetAccountByEmailOrPassword(string email, string password);
}