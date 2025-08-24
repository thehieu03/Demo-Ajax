namespace Entity.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public short AccountId { get; set; } 
    public string AccessTokenHash { get; set; } = null!;
    public string RefreshTokenHash { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public virtual SystemAccount? Account { get; set; }
}