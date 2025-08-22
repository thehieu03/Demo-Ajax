using Entity.ModelResponse;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DataAccess.DAO;

public static class NewsArticleDao
{
    private static readonly FunewsManagementContext Context = FunewsManagementContext.Instance;

    public static async Task<IEnumerable<NewsArticleResponse>> GetAll() =>
        await Context.NewsArticles
            .Include(c => c.CreatedBy)
            .Include(c => c.Category)
            .Select(a => new NewsArticleResponse
            {
                NewsArticleId = short.Parse(a.NewsArticleId),
                NewsTitle = a.NewsTitle ?? string.Empty,
                Headline = a.Headline,
                NewsContent = a.NewsContent ?? string.Empty,
                NewsSource = a.NewsSource,
                CategoryId = a.CategoryId,
                NewsStatus = a.NewsStatus,
                CategoryName = a.Category != null ? a.Category.CategoryName : string.Empty,
                AccountName = a.CreatedBy != null ? a.CreatedBy.AccountName : string.Empty
            })
            .ToListAsync();

    public static async Task<NewsArticle?> GetById(string id) =>
        await Context.NewsArticles.FindAsync(id);

    public static async Task Create(NewsArticle newsArticle)
    {
        Context.NewsArticles.Add(newsArticle);
        await Context.SaveChangesAsync();
    }

    public static async Task Update(NewsArticle newsArticle)
    {
        Context.NewsArticles.Update(newsArticle);
        await Context.SaveChangesAsync();
    }

    public static async Task Delete(string id)
    {
        var data = await Context.NewsArticles.FindAsync(id);
        if (data != null)
        {
            Context.NewsArticles.Remove(data);
            await Context.SaveChangesAsync();
        }
    }
}