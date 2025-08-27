using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO;

public static class TagDao
{
       public static async Task<IEnumerable<Tag>> GetAll()
       {
           using var context = new FunewsManagementContext();
           return await context.Tags.ToListAsync();
       }
       public static async Task<Tag?> GetById(int id)
       {
           using var context = new FunewsManagementContext();
           return await context.Tags.FindAsync(id);
       }

       public static async Task Create(Tag tag)
       {
           using var context = new FunewsManagementContext();
           context.Tags.Add(tag);
           await context.SaveChangesAsync();
       }

       public static async Task Update(Tag tag)
       {
           using var context = new FunewsManagementContext();
           context.Tags.Update(tag);
           await context.SaveChangesAsync();
       }

       public static async Task Delete(int tagId)
       {
           try
           {
               using var context = new FunewsManagementContext();
               var dataRemove = await context.Tags.FindAsync(tagId);
               if (dataRemove != null)
               {
                   var deletedRows = await context.Database.ExecuteSqlRawAsync(
                       "DELETE FROM NewsTag WHERE TagID = {0}", tagId);
                   
                   context.Tags.Remove(dataRemove);
                   await context.SaveChangesAsync();
               }
           }
           catch (Exception ex)
           {
               throw;
           }
       }
}