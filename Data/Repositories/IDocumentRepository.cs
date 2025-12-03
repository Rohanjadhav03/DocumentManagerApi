using DocumentManagerApi.Models;

namespace DocumentManagerApi.Data.Repositories
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetAllAsync(int page = 1, int pageSize = 20, string? search = null);
        Task<Document?> GetByIdAsync(int id);
        Task<int> CreateAsync(Document doc);
        Task<int> UpdateAsync(Document doc);
        Task<int> DeleteAsync(int id);
    }
}
