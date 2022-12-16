using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasPoem.Services
{
    public interface IAIService
    {
        Task<string> GetGreetingAsync();
        Task<string> GetPoemAsync(string keyword);
    }
}
