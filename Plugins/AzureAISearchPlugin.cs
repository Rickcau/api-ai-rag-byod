using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using api_ai_rag_byod.Interfaces;
using api_ai_rag_byod.Services;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace api_ai_rag_byod.Plugins
{
    public class AzureAISearchPlugin
    {
        private readonly IAzureAISearchService _searchService;
        #pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private readonly ITextEmbeddingGenerationService _textEmbeddingGenerationService;
        

        public AzureAISearchPlugin(ITextEmbeddingGenerationService textEmbeddingGenerationService, IAzureAISearchService searchService)
        {
            this._textEmbeddingGenerationService = textEmbeddingGenerationService;
            this._searchService = searchService;
        }
        #pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        
        [KernelFunction]
        [Description("When a user asks a question about manuals or help for the Healthcare this is the function you should execute")]
        public async Task<string> SearchAsync(
            string query,
            string collection,
            List<string>? searchFields = null,
            CancellationToken cancellationToken = default)
        {
            // Convert string query to vector
            #pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            ReadOnlyMemory<float> embedding = await this._textEmbeddingGenerationService.GenerateEmbeddingAsync(query, cancellationToken: cancellationToken);
            #pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            // Perform search
            return await this._searchService.SearchAsync(collection, embedding, searchFields, cancellationToken) ?? string.Empty;
        }
       

    }
}
