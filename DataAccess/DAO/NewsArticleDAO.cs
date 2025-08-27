using Entity.ModelResponse;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DataAccess.DAO;

public static class NewsArticleDao
{
    public static async Task<IEnumerable<NewsArticleResponse>> GetAll()
    {
        using var context = new FunewsManagementContext();
        return await context.NewsArticles
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
    }

    public static async Task<NewsArticle?> GetById(string id)
    {
        using var context = new FunewsManagementContext();
        return await context.NewsArticles.FindAsync(id);
    }

    public static async Task Create(NewsArticle newsArticle)
    {
        using var context = new FunewsManagementContext();
        context.NewsArticles.Add(newsArticle);
        await context.SaveChangesAsync();
    }

    public static async Task<NewsArticle> CreateNewsArticle(NewsArticle newsArticle)
    {
        using var context = new FunewsManagementContext();
        
        var allIds = await context.NewsArticles
            .AsNoTracking()
            .Select(x => x.NewsArticleId)
            .ToListAsync();

        var maxNumericId = allIds
            .Select(s => int.TryParse(s, out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();

        var nextId = maxNumericId + 1;
        newsArticle.NewsArticleId = nextId.ToString();

        if (!newsArticle.CreatedDate.HasValue)
        {
            newsArticle.CreatedDate = DateTime.UtcNow;
        }

        context.NewsArticles.Add(newsArticle);
        await context.SaveChangesAsync();
        return newsArticle;
    }

    public static async Task Update(NewsArticle newsArticle)
    {
        using var context = new FunewsManagementContext();
        context.NewsArticles.Update(newsArticle);
        await context.SaveChangesAsync();
    }

    public static async Task Delete(string id)
    {
        using var context = new FunewsManagementContext();
        var article = await context.NewsArticles
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.NewsArticleId == id);
        if (article != null)
        {
            if (article.Tags.Any())
            {
                article.Tags.Clear();
                await context.SaveChangesAsync();
            }

            context.NewsArticles.Remove(article);
            await context.SaveChangesAsync();
        }
    }
}