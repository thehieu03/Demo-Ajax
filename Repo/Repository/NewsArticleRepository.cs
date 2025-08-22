using DataAccess.DAO;
using Entity.ModelResponse;

namespace Repo.Repository;

public class NewsArticleRepository :INewsArticleRepository
{
    public Task<IEnumerable<NewsArticleResponse>> GetAllAsync() => NewsArticleDao.GetAll();

    public Task<NewsArticle> GetById(string id)=> NewsArticleDao.GetById(id);

    public Task Create(NewsArticle entity)=> NewsArticleDao.Create(entity);

    public Task Update(NewsArticle entity)=> NewsArticleDao.Update(entity);

    public Task Delete(string id)=> NewsArticleDao.Delete(id);
}