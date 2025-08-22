using DataAccess.DAO;
using Entity.ModelResponse;
using Entity.Models;

namespace Repo.Repository;

public class CategoryRepository :ICategoryRepository
{
    public Task<IEnumerable<CategoryResponse>> GetAllAsync()=>CategoryDao.GetCategories();

    public Task<Category> GetById(short id)=> CategoryDao.GetCategoryById(id);

    public Task Create(Category entity) => CategoryDao.CreateCategory(entity);

    public Task Update(Category entity)=> CategoryDao.UpdateCategory(entity);

    public Task Delete(short id)=> CategoryDao.DeleteCategory(id);
}