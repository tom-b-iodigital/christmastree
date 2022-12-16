using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChristmasPoem.Models
{
    public class AIRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "text-davinci-003";

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = "Write me a funny and short christmas poem";

        [JsonPropertyName("temperature")]
        public float Temperature { get; set; } = 0.7f;

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 200;
    }
}
