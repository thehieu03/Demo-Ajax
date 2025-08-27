using Entity.ModelResponse;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO;

public static class CategoryDao
{
    private static readonly FunewsManagementContext Context = FunewsManagementContext.Instance;

    public static async Task<IEnumerable<CategoryResponse>> GetCategories() =>
        await Context.Categories.Select(c => new CategoryResponse
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName ?? string.Empty,
            CategoryDesciption = c.CategoryDesciption ?? string.Empty,
            ParentCategoryId= c.ParentCategoryId,
            IsActive = c.IsActive,
            ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.CategoryName : null
        }).ToListAsync();

    public static async Task<Category?> GetCategoryById(short id) =>
        await Context.Categories.FindAsync(id);

    public static async Task CreateCategory(Category category)
    {
        Context.Categories.Add(category);
        await Context.SaveChangesAsync();
    }

    public static async Task UpdateCategory(Category category)
    {
        var existing = await Context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);
        if (existing == null)
        {
            return;
        }

        existing.CategoryName = category.CategoryName;
        existing.CategoryDesciption = category.CategoryDesciption;
        existing.ParentCategoryId = category.ParentCategoryId;
        existing.IsActive = category.IsActive;

        await Context.SaveChangesAsync();
    }

    public static async Task DeleteCategory(short id)
    {
        var category = await Context.Categories.FindAsync(id);
        if (category != null)
        {
            Context.Categories.Remove(category);
            await Context.SaveChangesAsync();
        }

    }
}