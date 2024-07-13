# api-ai-rag-byod
**Important Note** I am not using DevContainers because I have an issue with Docker that will requirement to reformat my laptop and I simply have not had time to do that.

This is a work in-progress and is an old example I started.  There are some parts of it that might be of use, but I would suggest you take a look at my [ai-rag-intent-blazor](https://github.com/Rickcau/ai-rag-intent-blazor) example for a much better example of Intent Recognition and RAG (chatting with your docs). The goal here is to demostrate the use of a RAG pattern using the latest versison of the Semantic Kernel via an Azure Function.  It leverages all the common patterns e.g. dependecy injection, SK Plugins/Functions, AutoInvoketion etc.  I still need to add Open API for a Swagger UI, some additional error handling and we need to clean up the AI Search and add logic for various types of AI Search.

Even in it's current state you can get an understand of how you would leverage a RAG pattern and a custom SK Plugin that allows you to perform DB Searches.  For the DB Search the idea is that you would leverage a DAL service that gives you access to the backend for query purposes.  The DAL would impliment all the permissions checks to ensure the chatuser has proper access to query for the data in-question.

Also, make note that the function requires you to pass a JSON boday with the following information:

        ~~~
              {
                 "userId": "stevesmith@contoso.com",
                 "sessionId": "12345678",
                 "tenantId": "00001",
                 "prompt": "Can you tell what my healtcare benefits are for Northwinds"
              }
        ~~~

The client that is calling the Function will pass these values in and later they can be used to store/retreive prompt history as well being passed to the DB Query plugin, which in-turn will pass this information to the DAL for permission and role purposes.  The tenantID would be used for scenarios in-which you had custom logic that needs to query data from a specific tenant, the tenantId would allow you to make sure you are targeting the tenantId that that use has access to.
