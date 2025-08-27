using System.Security.Claims;

namespace Server;

public static class ClaimsHelper
{
    public static bool TryGetAccountId(this ClaimsPrincipal? user, out int accountId)
    {
        accountId = 0;
        if (user?.Identity == null || !user.Identity.IsAuthenticated) return false;

        var raw = user.FindFirst("AccountId")?.Value
                  ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? user.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(raw)) return false;
        return int.TryParse(raw, out accountId);
    }
    public static short GetAccountId(this ClaimsPrincipal? user)
    {
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
            return 0;

        var raw = user.FindFirst(ClaimTypes.Sid)?.Value
                  ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? user.FindFirst("sub")?.Value;

        return short.TryParse(raw, out var accountId) ? accountId : (short)0;
    }
}