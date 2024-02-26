using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using api_ai_rag_byod.Plugins;
using Azure;
using System.Text.Json;
using api_ai_rag_byod.Models;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Net;

namespace api_ai_rag_byod.Functions
{
    public class ChatProvider
    {
        private readonly ILogger<ChatProvider> _logger;
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chat;
        private readonly ChatHistory _chatHistory;

        public ChatProvider(ILogger<ChatProvider> logger, Kernel kernel, IChatCompletionService chat, ChatHistory chatHistory)
        {
            _logger = logger;
            _kernel = kernel;
            _chat = chat;
            _chatHistory = chatHistory;
            // _kernel.ImportPluginFromObject(new TextAnalyticsPlugin(_client));
        }

        [Function("ChatProvider")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            // Request body example:
            /*
                {
                    "userId": "stevesmith@contoso.com",
                    "sessionId": "12345678",
                    "tenantId": "00001",
                    "prompt": "Hello, What can you do for me?"
                }
            */


            _logger.LogInformation("C# HTTP SentimentAnalysis trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var chatRequest = JsonSerializer.Deserialize<ChatProviderRequest>(requestBody);
            if (chatRequest == null || chatRequest.userId == null || chatRequest.sessionId == null || chatRequest.tenantId == null || chatRequest.prompt == null)
            {
                throw new ArgumentNullException("Please check your request body, you are missing required data.");
            }

            /* test
            var arguments = new KernelArguments { ["searchFields"] = JsonSerializer.Serialize(new List<string> { "contentVector" }) };
            var result2 = await _kernel.InvokePromptAsync(
                            "{{search 'content' collection='rr-demo-index' searchFields=$searchFields}} What are my healthcare benefits for Northwinds?",
                             arguments);

            */

            _chatHistory.AddMessage(AuthorRole.User, chatRequest.prompt);
            // now it's time to use the Kernel to invoke our logic...
            // lets call the Chat Completion without using RAG for now...
            var result = await _chat.GetChatMessageContentAsync(
                    _chatHistory,
                    executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 800, Temperature = 0.7, TopP = 0.0, ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions },
                    kernel: _kernel);
            
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            try
            {
                await response.WriteStringAsync(result.Content!);
            }
            catch (Exception ex)
            {
                // Log exception details here
                throw; // Re-throw the exception to propagate it further
            }

            return response;
        }
    }
}
