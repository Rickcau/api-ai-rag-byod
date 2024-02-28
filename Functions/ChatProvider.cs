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
using Microsoft.SemanticKernel.Http;
using System.Net;
using api_ai_rag_byod.Util;
using System.Net.NetworkInformation;

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


            
            // We are going to call the SearchPlugin to see if we get any hits on the query, if we do add them to the chat history and let AI summarize it 

            // var function = _kernel.Plugins.GetFunction("AzureAISearchPlugin", "SimpleHybridSearch"); 
            var function = _kernel.Plugins.GetFunction("AzureAISearchPlugin", "SemanticHybridSearch");

            var responseContent = await _kernel.InvokeAsync(function, new() { ["query"] = chatRequest.prompt });

            var promptTemplate = $"{responseContent.ToString()}\n Using the details above attempt to summarize or answer to the following question \n Question: {chatRequest.prompt} \n if you cannot complete the task using the above information, do not use external knowledge and simply state you cannot help with that question";
            _chatHistory.AddMessage(AuthorRole.User, promptTemplate);

            // _chatHistory.AddMessage(AuthorRole.User, responseTest.ToString());
            // _chatHistory.AddMessage(AuthorRole.User, chatRequest.prompt);
            // _chatHistory.AddMessage(AuthorRole.System, "If the prompt cannot be answered by the AzureAISearchPlugin or the DBQueryPlugin, then simply ask for more details");

            // now it's time to use the Kernel to invoke our logic...
            // lets call the Chat Completion without using RAG for now...
            var result = await _chat.GetChatMessageContentAsync(
                    _chatHistory,
                    executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 800, Temperature = 0.7, TopP = 0.0, ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions },
                    kernel: _kernel);
          

            //  var func = _kernel.Plugins.TryGetFunction("AzureAISearchPlugin","SimpleHybridSearch", out function);
            
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

        [Function("ChatProviderv2")]
        public async Task<HttpResponseData> Run2([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            // This approach needs some tuning 
            string _promptSummarize = @"SUMMARIZE THE QUERY REFERENCED IN THE USER SECTION BELOW IN 10 BULLET POINTS OR LESS

            SUMMARY MUST BE:
            - WORKPLACE / FAMILY SAFE NO SEXISM, RACISM OR OTHER BIAS/BIGOTRY
            - G RATED
            - IF THERE ARE ANY  INCONSISTENCIES, DO YOUR BEST TO CALL THOSE OUT
            
            RESULTS:
            - IF THE RESULTS FROM THE AzureAISearchPlugin are not related to the query simply state you do not have enough information

            User:{{AzureAISearchPlugin.SemanticHybridSearch $query}}

            Assistant: ";

            _logger.LogInformation("C# HTTP SentimentAnalysis trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var chatRequest = JsonSerializer.Deserialize<ChatProviderRequest>(requestBody);
            if (chatRequest == null || chatRequest.userId == null || chatRequest.sessionId == null || chatRequest.tenantId == null || chatRequest.prompt == null)
            {
                throw new ArgumentNullException("Please check your request body, you are missing required data.");
            }

            // We are going to call the SearchPlugin to see if we get any hits on the query, if we do add them to the chat history and let AI summarize it 

            var func = _kernel.CreateFunctionFromPrompt(_promptSummarize);
            var responseContent = await _kernel.InvokeAsync(func, new() { ["query"] = chatRequest.prompt });

            var promptTemplate = $"{responseContent.ToString()}\n Using the details above attempt to summarize or answer to the following question \n Question: {chatRequest.prompt} \n if you cannot complete the task using the above information, do not use external knowledge and simply state you cannot help with that question";

            _chatHistory.AddMessage(AuthorRole.User, responseContent.ToString());

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
