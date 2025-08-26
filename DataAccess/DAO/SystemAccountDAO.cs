using Entity.ModelResponse;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO;

public static class SystemAccountDao
{
    private static readonly FunewsManagementContext Context = FunewsManagementContext.Instance;

    public static async Task<IEnumerable<SystemAccountResponse>> GetAll()
    {
        return await Context.SystemAccounts.Select(c => new SystemAccountResponse()
        {
            AccountName = c.AccountName,
            AccountEmail = c.AccountEmail,
            AccountRole = c.AccountRole
        }).ToListAsync();
    }

    public static async Task<SystemAccount?> GetById(short id)
    {
        return await Context.SystemAccounts.FindAsync(id);
    }

    public static async Task Create(SystemAccount systemAccount)
    {
        Context.SystemAccounts.Add(systemAccount);
        await Context.SaveChangesAsync();
    }

    public static async Task Update(SystemAccount systemAccount)
    {
        Context.SystemAccounts.Update(systemAccount);
        await Context.SaveChangesAsync();
    }

    public static async Task Delete(short id)
    {
        var data = await Context.SystemAccounts.FindAsync(id);
        if (data != null)
        {
            Context.SystemAccounts.Remove(data);
            await Context.SaveChangesAsync();
        }
    }

    public static async Task<SystemAccount?> Login(string email, string password)
    {
        return await Context.SystemAccounts
            .FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);
    }
    public static async Task<SystemAccount?> GetByEmail(string email)
    {
        var data = await Context.SystemAccounts.FirstOrDefaultAsync(a => a.AccountEmail.Equals(email));
        return data;
    }
}