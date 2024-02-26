using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_ai_rag_byod.Models
{
    public class ChatProviderResponseDto
    {
        public string? StatusMessage { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? Result { get; set; } = string.Empty;
    }
}
