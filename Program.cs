
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Memory;
using api_ai_rag_byod.Util;
using api_ai_rag_byod.Plugins;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using api_ai_rag_byod.Interfaces;
using api_ai_rag_byod.Services;
using Microsoft.Win32;

//
string _apiDeploymentName = Helper.GetEnvironmentVariable("ApiDeploymentName");
string _apiEndpoint = Helper.GetEnvironmentVariable("ApiEndpoint");
string _apiKey = Helper.GetEnvironmentVariable("ApiKey");
string _apiAISearchEndpoint = Helper.GetEnvironmentVariable("AISearchURL"); 
string _apiAISearchKey= Helper.GetEnvironmentVariable("AISearchKey"); 
string _textEmbeddingName = Helper.GetEnvironmentVariable("EmbeddingName");

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddTransient<Kernel>(s =>
        {
            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                _apiDeploymentName,
                _apiEndpoint,
                _apiKey
                );
            builder.Services.AddSingleton<SearchIndexClient>(s =>
            {
                string endpoint = _apiAISearchEndpoint;
                string apiKey = _apiAISearchKey;
                return new SearchIndexClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));
            });

            // Custom AzureAISearchService to configure request parameters and make a request.
            builder.Services.AddSingleton<IAzureAISearchService, AzureAISearchService>();
            
            #pragma warning disable SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            builder.AddAzureOpenAITextEmbeddingGeneration(_textEmbeddingName, _apiEndpoint, _apiKey);
            #pragma warning restore SKEXP0011 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            
            builder.Plugins.AddFromType<DBQueryPlugin>();
            builder.Plugins.AddFromType<AzureAISearchPlugin>();
            
            return builder.Build();
        });

        services.AddSingleton<IChatCompletionService>(sp =>
                     sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());
        const string systemmsg = "You are a helpful friendly assistant that has knowledge of Org Builder Manuals.  You also have the ability to perform Org Build Database queries.  Do not answer any questions related to custom plugins or anything that is not related to the manuals or querying of the Org Builder Database.";
        services.AddSingleton<ChatHistory>(s =>
        {
            var chathistory = new ChatHistory();
            chathistory.AddAssistantMessage(systemmsg);
            return chathistory;
        });

    })
    .Build();

host.Run();

