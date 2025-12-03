using System.Data;
using System.Data.SqlClient;
using DocumentManagerApi.Models;

namespace DocumentManagerApi.Data.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IDatabaseHelper _db;

        public DocumentRepository(IDatabaseHelper db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Document>> GetAllAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            var offset = (page - 1) * pageSize;
            // Simple search and pagination example
            var sql = @"SELECT Id, CaseId, Title, Tags, Content, CreatedBy, CreatedAt, UpdatedAt
                        FROM Documents
                        WHERE (@search IS NULL OR Title LIKE '%' + @search + '%' OR Tags LIKE '%' + @search + '%')
                        ORDER BY CreatedAt DESC
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            var dt = await _db.ExecuteDataTableAsync(sql,
                new SqlParameter("@search", SqlDbType.NVarChar) { Value = (object?)search ?? DBNull.Value },
                new SqlParameter("@offset", SqlDbType.Int) { Value = offset },
                new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

            var list = new List<Document>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Document
                {
                    Id = Convert.ToInt32(row["Id"]),
                    CaseId = Convert.ToInt32(row["CaseId"]),
                    Title = Convert.ToString(row["Title"]) ?? string.Empty,
                    Tags = Convert.ToString(row["Tags"]),
                    Content = Convert.ToString(row["Content"]),
                    CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                    UpdatedAt = row["UpdatedAt"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["UpdatedAt"])
                });
            }
            return list;
        }

        public async Task<Document?> GetByIdAsync(int id)
        {
            var sql = "SELECT Id, CaseId, Title, Tags, Content, CreatedBy, CreatedAt, UpdatedAt FROM Documents WHERE Id = @id";
            var dt = await _db.ExecuteDataTableAsync(sql, new SqlParameter("@id", SqlDbType.Int) { Value = id });
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new Document
            {
                Id = Convert.ToInt32(row["Id"]),
                CaseId = Convert.ToInt32(row["CaseId"]),
                Title = Convert.ToString(row["Title"]) ?? string.Empty,
                Tags = Convert.ToString(row["Tags"]),
                Content = Convert.ToString(row["Content"]),
                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                UpdatedAt = row["UpdatedAt"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["UpdatedAt"])
            };
        }

        public async Task<int> CreateAsync(Document doc)
        {
            var sql = @"INSERT INTO Documents (CaseId, Title, Tags, Content, CreatedBy, CreatedAt)
                        VALUES (@CaseId, @Title, @Tags, @Content, @CreatedBy, @CreatedAt);
                        SELECT SCOPE_IDENTITY();";

            var result = await _db.ExecuteScalarAsync(sql,
                new SqlParameter("@CaseId", SqlDbType.Int) { Value = doc.CaseId },
                new SqlParameter("@Title", SqlDbType.NVarChar) { Value = doc.Title },
                new SqlParameter("@Tags", SqlDbType.NVarChar) { Value = (object?)doc.Tags ?? DBNull.Value },
                new SqlParameter("@Content", SqlDbType.NVarChar) { Value = (object?)doc.Content ?? DBNull.Value },
                new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = doc.CreatedBy },
                new SqlParameter("@CreatedAt", SqlDbType.DateTime) { Value = doc.CreatedAt });

            return Convert.ToInt32(result ?? 0);
        }

        public async Task<int> UpdateAsync(Document doc)
        {
            var sql = @"UPDATE Documents SET CaseId=@CaseId, Title=@Title, Tags=@Tags, Content=@Content, UpdatedAt=@UpdatedAt
                        WHERE Id=@Id";

            return await _db.ExecuteNonQueryAsync(sql,
                new SqlParameter("@CaseId", SqlDbType.Int) { Value = doc.CaseId },
                new SqlParameter("@Title", SqlDbType.NVarChar) { Value = doc.Title },
                new SqlParameter("@Tags", SqlDbType.NVarChar) { Value = (object?)doc.Tags ?? DBNull.Value },
                new SqlParameter("@Content", SqlDbType.NVarChar) { Value = (object?)doc.Content ?? DBNull.Value },
                new SqlParameter("@UpdatedAt", SqlDbType.DateTime) { Value = DateTime.UtcNow },
                new SqlParameter("@Id", SqlDbType.Int) { Value = doc.Id });
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Documents WHERE Id = @Id";
            return await _db.ExecuteNonQueryAsync(sql, new SqlParameter("@Id", SqlDbType.Int) { Value = id });
        }
    }
}
