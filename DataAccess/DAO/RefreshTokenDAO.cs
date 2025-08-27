using Entity.Models;

namespace DataAccess.DAO;

public static class RefreshTokenDAO
{
    public static async Task<RefreshToken?> GetByAccountIdAsync(string accountId)
    {
        using var context = new FunewsManagementContext();
        return await context.RefreshTokens.FindAsync(accountId);
    }
}