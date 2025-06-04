using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IOpenAiService
    {
        Task<string> GetChatResponseAsync(string prompt);
    }

}
