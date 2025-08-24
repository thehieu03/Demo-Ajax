namespace Entity.ModelResponse;

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public int AccountRole { get; set; }
    public short AccountId { get; set; }
}