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
           try
           {
               Console.WriteLine($"[DAO_DELETE_TAG] Bắt đầu xóa tag với ID: {tagId}");
               
               var dataRemove = await Context.Tags.FindAsync(tagId);
               if (dataRemove != null)
               {
                   Console.WriteLine($"[DAO_DELETE_TAG] Tìm thấy tag: {dataRemove.TagName}");
                   
                   // Bước 1: Xóa các liên kết trong bảng NewsTag bằng raw SQL
                   Console.WriteLine($"[DAO_DELETE_TAG] Bắt đầu xóa liên kết NewsTag bằng raw SQL");
                   
                   var deletedRows = await Context.Database.ExecuteSqlRawAsync(
                       "DELETE FROM NewsTag WHERE TagID = {0}", tagId);
                   
                   Console.WriteLine($"[DAO_DELETE_TAG] Đã xóa {deletedRows} liên kết NewsTag bằng raw SQL");
                   
                   // Bước 2: Sau đó xóa Tag
                   Console.WriteLine($"[DAO_DELETE_TAG] Bắt đầu remove Tag từ context");
                   Context.Tags.Remove(dataRemove);
                   Console.WriteLine($"[DAO_DELETE_TAG] Đã remove Tag, bắt đầu SaveChangesAsync");
                   
                   await Context.SaveChangesAsync();
                   Console.WriteLine($"[DAO_DELETE_TAG] SaveChangesAsync thành công - Tag đã được xóa hoàn toàn");
               }
               else
               {
                   Console.WriteLine($"[DAO_DELETE_TAG] Không tìm thấy tag với ID: {tagId}");
               }
           }
           catch (Exception ex)
           {
               Console.WriteLine($"[DAO_DELETE_TAG] Lỗi trong Delete: {ex.Message}");
               Console.WriteLine($"[DAO_DELETE_TAG] Stack trace: {ex.StackTrace}");
               
               if (ex.InnerException != null)
               {
                   Console.WriteLine($"[DAO_DELETE_TAG] Inner exception: {ex.InnerException.Message}");
                   Console.WriteLine($"[DAO_DELETE_TAG] Inner stack trace: {ex.InnerException.StackTrace}");
               }
               
               throw;
           }
       }
}