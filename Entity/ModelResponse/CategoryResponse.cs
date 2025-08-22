namespace Entity.ModelResponse;

public class CategoryResponse
{
    public short CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;

    public string CategoryDesciption { get; set; } = null!;
}