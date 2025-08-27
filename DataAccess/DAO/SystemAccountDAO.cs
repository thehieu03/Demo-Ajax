using Entity.ModelResponse;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DataAccess.DAO;

public static class SystemAccountDao
{
    public static async Task<IEnumerable<SystemAccountResponse>> GetAll()
    {
        using var context = new FunewsManagementContext();
        return await context.SystemAccounts.Select(c => new SystemAccountResponse()
        {
            AccountName = c.AccountName,
            AccountEmail = c.AccountEmail,
            AccountRole = c.AccountRole
        }).ToListAsync();
    }

    public static async Task<SystemAccount?> GetById(short id)
    {
        using var context = new FunewsManagementContext();
        return await context.SystemAccounts.FindAsync(id);
    }

    public static async Task Create(SystemAccount systemAccount)
    {
        try
        {
            using var context = new FunewsManagementContext();
            
            if (systemAccount.AccountId == 0)
            {
                var maxId = await context.SystemAccounts.MaxAsync(a => (short?)a.AccountId) ?? 0;
                systemAccount.AccountId = (short)(maxId + 1);
            }
            
            context.SystemAccounts.Add(systemAccount);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public static async Task Update(SystemAccount systemAccount)
    {
        using var context = new FunewsManagementContext();
        context.SystemAccounts.Update(systemAccount);
        await context.SaveChangesAsync();
    }

    public static async Task Delete(short id)
    {
        using var context = new FunewsManagementContext();
        var data = await context.SystemAccounts.FindAsync(id);
        if (data != null)
        {
            context.SystemAccounts.Remove(data);
            await context.SaveChangesAsync();
        }
    }

    public static async Task<SystemAccount?> Login(string email, string password)
    {
        using var context = new FunewsManagementContext();
        return await context.SystemAccounts
            .FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);
    }
    public static async Task<SystemAccount?> GetByEmail(string email)
    {
        using var context = new FunewsManagementContext();
        var data = await context.SystemAccounts.FirstOrDefaultAsync(a => a.AccountEmail.Equals(email));
        return data;
    }
    
    public static async Task<short> GetNextAccountId()
    {
        try
        {
            using var context = new FunewsManagementContext();
            var maxId = await context.SystemAccounts.MaxAsync(a => (short?)a.AccountId) ?? 0;
            return (short)(maxId + 1);
        }
        catch (Exception ex)
        {
            return 1;
        }
    }
}