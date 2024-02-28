using api_ai_rag_byod.Models;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_ai_rag_byod.Util
{
    public class HelperHTTP
    {
        string mytest = "this is a test";
        

        public string Prompt { get; set; } = string.Empty;
        public RootObject? RootObject { get; set; } 

        public void CreateDataSource()
        {
            RootObject = new RootObject
            {
                Temperature = 0,
                MaxTokens = 1000,
                TopP = 1.0,
                DataSources = new List<DataSource>
                {
                new DataSource
                    {
                        Type = "AzureCognitiveSearch",
                        Parameters = new DataSourceParameters
                        {
                            Endpoint = "https://orgbuildersearchservice.search.windows.net",
                            Key = "DTjvT82mDxreV5CwcssRLFnoZn6HrlRJPFAT70G7OwAzSeBSx7Fz",
                            IndexName = "obvector-1709061059373"
                        }
                    }
                },
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = "user",
                        Content = Prompt
                    }
                }
            };
        }


    }
}
