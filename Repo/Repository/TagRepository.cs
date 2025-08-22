namespace Repo.Repository;

public class TagRepository: ITagRepository
{
    public Task<IEnumerable<Tag>> GetAllAsync()=>TagDao.GetAll();

    public Task<Tag> GetById(int id)=> TagDao.GetById(id);

    public Task Create(Tag entity)=> TagDao.Create(entity);

    public Task Update(Tag entity)=> TagDao.Update(entity);

    public Task Delete(int id)=>TagDao.Delete(id);
    
}