using Entity.ModelResponse;

namespace Repo.Repository;

public interface INewsArticleRepository :IGenericRepository<NewsArticle,NewsArticleResponse, string>
{
    
}