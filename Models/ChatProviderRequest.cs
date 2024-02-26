using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_ai_rag_byod.Models
{
    public class ChatProviderRequest
    {
        public string? userId { get; set; } = string.Empty;
        public string? sessionId { get; set; } = string.Empty;
        public string? tenantId { get; set; } = string.Empty;
        public string? prompt { get; set; } = string.Empty;
       
    }
}
