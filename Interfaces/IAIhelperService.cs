using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api_ai_rag_byod.Services;

namespace api_ai_rag_byod.Interfaces
{
    public interface IAIhelperService
    {
        Task<string> GetTranslationAsync(string input);
    }
}
