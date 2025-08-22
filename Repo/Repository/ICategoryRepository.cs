using Entity.ModelResponse;
using Entity.Models;

namespace Repo.Repository;

public interface ICategoryRepository: IGenericRepository<Category,CategoryResponse, short>
{
    
}