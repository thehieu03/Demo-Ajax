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

    public static async Task<NewsArticle> CreateNewsArticle(NewsArticle newsArticle)
    {
        var allIds = await Context.NewsArticles
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

        Context.NewsArticles.Add(newsArticle);
        await Context.SaveChangesAsync();
        return newsArticle;
    }

    public static async Task Update(NewsArticle newsArticle)
    {
        Context.NewsArticles.Update(newsArticle);
        await Context.SaveChangesAsync();
    }

    public static async Task Delete(string id)
    {
        var article = await Context.NewsArticles
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.NewsArticleId == id);
        if (article != null)
        {
            if (article.Tags.Any())
            {
                article.Tags.Clear();
                await Context.SaveChangesAsync();
            }

            Context.NewsArticles.Remove(article);
            await Context.SaveChangesAsync();
        }
    }
}