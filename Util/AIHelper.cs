using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Collections.Specialized;
using api_ai_rag_byod.Interfaces;

namespace api_ai_rag_byod.Util
{
    public class AIHelper : IAIhelperService
    {
        public Kernel _kernel;
        private List<string> _promptCollection = new List<string>();

        public int TenantId { get; set; }
   
        private string _promptPrompt1 = @"Translate the input below into spanish

        MAKE SURE YOU ONLY USE SPANISH.

        {{$input}}

        Translation:";

        private string _promptPrompt2 = @"Translate the input below into French

        MAKE SURE YOU ONLY USE FRENCH.

        {{$input}}

        Translation:";

        private string _promptPrompt3 = @"Translate the input below into French

        MAKE SURE YOU ONLY USE FRENCH.

        {{$input}}

        Translation:";


        private string _prompt3 = @"SUMMARIZE THE QUERY REFERENCED IN THE USER SECTION BELOW IN 10 BULLET POINTS OR LESS

            SUMMARY MUST BE:
            - WORKPLACE / FAMILY SAFE NO SEXISM, RACISM OR OTHER BIAS/BIGOTRY
            - G RATED
            - IF THERE ARE ANY  INCONSISTENCIES, DO YOUR BEST TO CALL THOSE OUT
            
            RESULTS:
            - IF THE RESULTS FROM THE AzureAISearchPlugin are not related to the query simply state you do not have enough information

            User:{{DBQueryPlugin.GetHRContact $query}}

            Assistant: ";


        public AIHelper()
        {
            // Add the string variables to the list
            _promptCollection.Add(_promptPrompt1);
            _promptCollection.Add(_promptPrompt2);
        }

        public async Task<string> GetTranslationAsync(string input)
        {
            var theFunction = _kernel.CreateFunctionFromPrompt(_promptCollection[this.TenantId], executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 100 });

            var response = await _kernel.InvokeAsync(theFunction);
            // new() { ["input"] = input }
            return response.ToString();
        }


    }
}
