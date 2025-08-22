using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO;

public static class TagDao
{
    private static readonly FunewsManagementContext Context = FunewsManagementContext.Instance;
       public static async Task<IEnumerable<Tag>> GetAll()
       {
           return await Context.Tags.ToListAsync();
       }
       public static async Task<Tag?> GetById(int id)
       {
           return await Context.Tags.FindAsync(id);
       }

       public static async Task Create(Tag tag)
       {
           Context.Tags.Add(tag);
           await Context.SaveChangesAsync();
       }

       public static async Task Update(Tag tag)
       {
           Context.Tags.Update(tag);
           await Context.SaveChangesAsync();
       }

       public static async Task Delete(int tagId)
       {
           var dataRemove = await Context.Tags.FindAsync(tagId);
           if (dataRemove != null)
           {
               Context.Tags.Remove(dataRemove);
               await Context.SaveChangesAsync();
           }
       }
}