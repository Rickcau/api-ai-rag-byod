
using Azure;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI;
using System.ComponentModel;
using api_ai_rag_byod.Models;
using api_ai_rag_byod.Interfaces;
using api_ai_rag_byod.Util;
using Microsoft.SemanticKernel.Embeddings;

namespace api_ai_rag_byod.Plugins
{
    public class DBQueryPlugin
    {
        private readonly AIHelper _aiHelper;
        private static bool _hrToggleContact;

        public DBQueryPlugin(AIHelper aiHelper)
        {
            this._aiHelper = aiHelper;
        }

        [KernelFunction]
        [Description("Allows a user find who their HR contact is based on the location that they pass in.  If Location is not provided then set it to null")]
        public static string GetHRContact([Description("Return who the HR Contact is for the user")] string query, string location)
        {
           
            if (location == null)
            {
                return "Please provide the location information?";
            }
            
            if (_hrToggleContact)
            {
                _hrToggleContact = false;
                return "Your HR contact is Steve Smith";
            }
            else
            {
                _hrToggleContact= true;
                return "Your HR contact is Lisa Jones";
            }
        }

        [KernelFunction]
        [Description("Allow the user to search the Organization for the IT Manager ")]
        public static string TurnLightOff([Description("Who is the IT manager")] bool lightStatus)
        {
            return "Your IT manager is David Hampton.";
        }

        [KernelFunction]
        [Description("This function allows the user to request that you translate the input into a language based on the tenant they below to. ")]
        public async Task<string> TranslateInputToLanguage([Description("Return who the HR Contact is for the user")] string input)
        {
            var translation = await _aiHelper.GetTranslationAsync(input);
            return translation ?? "";
        }

    }
}
