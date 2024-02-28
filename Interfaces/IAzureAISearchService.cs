using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api_ai_rag_byod.Services;

namespace api_ai_rag_byod.Interfaces
{
    public interface IAzureAISearchService
    {
        Task<string?> SearchAsync(
            string collectionName,
            ReadOnlyMemory<float> vector,
            List<string>? searchFields = null,
            CancellationToken cancellationToken = default);
        Task<string> SimpleHybridSearchAsync(ReadOnlyMemory<float> embedding, string query, int k = 3);
        Task<string> SemanticHybridSearchAsync(ReadOnlyMemory<float> embedding, string query, int k = 3);
    }
}
