using Newtonsoft.Json;

namespace BlindRainGames.Utils.VisionBot
{
    /// <summary>
    /// Class to deserialize json respond from server.
    /// </summary>
    public class VBResponse
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("qr")]
        public string QR { get; set; }
    }
}
//EndFile//