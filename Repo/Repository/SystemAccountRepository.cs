using DataAccess.DAO;
using Entity.ModelResponse;

namespace Repo.Repository;

public class SystemAccountRepository   :ISystemAccountRepository
{
    public Task<IEnumerable<SystemAccountResponse>> GetAllAsync()=> SystemAccountDao.GetAll();

    public Task<SystemAccount> GetById(short id)=> SystemAccountDao.GetById(id);

    public Task Create(SystemAccount entity)=> SystemAccountDao.Create(entity);

    public Task Update(SystemAccount entity)=> SystemAccountDao.Update(entity);

    public Task Delete(short id)=> SystemAccountDao.Delete(id);

    public Task<SystemAccount?> GetAccountByEmailOrPassword(string email, string password) =>
        SystemAccountDao.Login(email, password);

    public Task<SystemAccount?> GetByEmail(string email)=> SystemAccountDao.GetByEmail(email);
}