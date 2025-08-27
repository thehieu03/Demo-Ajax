using Entity.ModelResponse;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        try
        {
            Console.WriteLine($"[DAO_CREATE] Bắt đầu tạo SystemAccount:");
            Console.WriteLine($"  - AccountId: {systemAccount.AccountId}");
            Console.WriteLine($"  - AccountName: {systemAccount.AccountName}");
            Console.WriteLine($"  - AccountEmail: {systemAccount.AccountEmail}");
            Console.WriteLine($"  - AccountRole: {systemAccount.AccountRole}");
            Console.WriteLine($"  - AccountPassword: {systemAccount.AccountPassword}");
            
            // Tự động tạo ID mới nếu AccountId = 0
            if (systemAccount.AccountId == 0)
            {
                var maxId = await Context.SystemAccounts.MaxAsync(a => (short?)a.AccountId) ?? 0;
                systemAccount.AccountId = (short)(maxId + 1);
                Console.WriteLine($"[DAO_CREATE] Đã tạo ID mới: {systemAccount.AccountId}");
            }
            
            Context.SystemAccounts.Add(systemAccount);
            Console.WriteLine("[DAO_CREATE] Đã thêm entity vào context");
            
            await Context.SaveChangesAsync();
            Console.WriteLine("[DAO_CREATE] SaveChangesAsync thành công");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DAO_CREATE] Lỗi trong Create: {ex.Message}");
            Console.WriteLine($"[DAO_CREATE] Stack trace: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[DAO_CREATE] Inner exception: {ex.InnerException.Message}");
                Console.WriteLine($"[DAO_CREATE] Inner stack trace: {ex.InnerException.StackTrace}");
            }
            
            throw;
        }
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
    
    public static async Task<short> GetNextAccountId()
    {
        try
        {
            var maxId = await Context.SystemAccounts.MaxAsync(a => (short?)a.AccountId) ?? 0;
            return (short)(maxId + 1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DAO_GET_NEXT_ID] Lỗi khi lấy ID tiếp theo: {ex.Message}");
            return 1; // Trả về 1 nếu có lỗi
        }
    }
}