using Entity.Models;

namespace DataAccess.DAO;

public static class RefreshTokenDAO
{
    private static readonly FunewsManagementContext Context = FunewsManagementContext.Instance;
    public static async Task<RefreshToken?> GetByAccountIdAsync(string accountId)
    {
        return await Context.RefreshTokens.FindAsync(accountId);
    }
}