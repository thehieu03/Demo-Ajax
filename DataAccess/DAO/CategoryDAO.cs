using Entity.ModelResponse;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO;

public static class CategoryDao
{
    public static async Task<IEnumerable<CategoryResponse>> GetCategories()
    {
        using var context = new FunewsManagementContext();
        return await context.Categories.Select(c => new CategoryResponse
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName ?? string.Empty,
            CategoryDesciption = c.CategoryDesciption ?? string.Empty,
            ParentCategoryId= c.ParentCategoryId,
            IsActive = c.IsActive,
            ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.CategoryName : null
        }).ToListAsync();
    }

    public static async Task<Category?> GetCategoryById(short id)
    {
        using var context = new FunewsManagementContext();
        return await context.Categories.FindAsync(id);
    }

    public static async Task CreateCategory(Category category)
    {
        using var context = new FunewsManagementContext();
        context.Categories.Add(category);
        await context.SaveChangesAsync();
    }

    public static async Task UpdateCategory(Category category)
    {
        using var context = new FunewsManagementContext();
        var existing = await context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);
        if (existing == null)
        {
            return;
        }

        existing.CategoryName = category.CategoryName;
        existing.CategoryDesciption = category.CategoryDesciption;
        existing.ParentCategoryId = category.ParentCategoryId;
        existing.IsActive = category.IsActive;

        await context.SaveChangesAsync();
    }

    public static async Task DeleteCategory(short id)
    {
        using var context = new FunewsManagementContext();
        var category = await context.Categories.FindAsync(id);
        if (category != null)
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }
    }
}