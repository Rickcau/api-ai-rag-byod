
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

namespace api_ai_rag_byod.Plugins
{
    public class DBQueryPlugin
    {

        private static bool _hrToggleContact;

        [KernelFunction]
        [Description("Allows a user find who their HR contact is.")]
        public static string GetHRContact([Description("Return who the HR Contact is for the user")] string query)
        {
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
    }
}
