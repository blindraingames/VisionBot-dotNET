/// Copyright 2022 by lead developer of studio "BlindRainGames" (https://blindraingames.ru) - Denis Lomakin.
/// Published under GPL 3.0 license.
/// All trademarks and software of VisionBot is not mine. Refer to https://visionbot.ru for details.
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlindRainGames.Utils.VisionBot
{
    /// <summary>
    /// Class for send image to VisionBot.ru for optical recognition.
    /// </summary>
    public class VisionBot : IDisposable
    {
        // Url for sending image
        protected string _imageURL = "https://visionbot.ru/apiv2/in.php";
        // Url for receive text
        protected string _resultURL = "https://visionbot.ru/apiv2/res.php";
        protected readonly HttpClient _client = new HttpClient();

        public VisionBot() { }
        public VisionBot(string img_url, string result_url)
        {
            _imageURL = img_url;
            _resultURL = result_url;
        }

        /// <summary>
        /// Method sends image to the VisionBot (https://visionbot.ru) server for recognition and waits for result.
        /// </summary>
        /// <param name="image"> Image to recognize in bytes array format. </param>
        /// <param name="recognition_type">Type of recognition. Possible values: all image text</param>
        /// <param name="is_translate">Should VisionBot try to translate response text?</param>
        /// <param name="lang">Language in which response will come.</param>
        /// <param name="is_qr">Should VisionBot try to recognize QR-codes?</param>
        /// <param name="delay">Delay in miliseconds after which request repeats.</param>
        /// <param name="tries_count">Count of tries after which request fails.</param>
        /// <returns> Description of image, qr code translation. If request fails, returns empty string. </returns>
        public async Task<string> GetImageDescriptionAsStringAsync(byte[] image, ERecognitionType recognition_type = ERecognitionType.all, bool is_translate = false, string lang = "en", bool is_qr = true, int delay = 1000, int tries_count = 30)
        {
            var response = await SendImageAsync(image, recognition_type.ToString(), is_translate, lang, is_qr);
            if (response.Status == "ok")
            {
                while (tries_count >= 0)
                {
                    await Task.Delay(delay);
                    response = await RequestStatusAsync(response.ID);
                    tries_count--;
                    if (response.Status == "ok")
                        return $"{response.Text}\r\n{response.QR}";
                    else if (response.Status == "error")
                        return String.Empty;
                } //while
            } //if
              //else
            return String.Empty;
        }

        /// <summary>
        /// Method sends image to the VisionBot (https://visionbot.ru) server for recognition.
        /// </summary>
        /// <param name="image"> Image to recognize in bytes array format. </param>
        /// <param name="recognition_type">Type of recognition. Possible values: all image text</param>
        /// <param name="is_translate">Should VisionBot try to translate response text?</param>
        /// <param name="lang">Language in which response will come.</param>
        /// <param name="is_qr">Should VisionBot try to recognize QR-codes?</param>
        /// <returns> id and status of recognition process. If request fails, returns object with status == "error". </returns>
        public async Task<VBResponse> SendImageAsync(byte[] image, string recognition_type = "all", bool is_translate = false, string lang = "en", bool is_qr = true)
        {
            // Prepare data to send
            using (var form = new MultipartFormDataContent())
            {
                form.Add(new StringContent(Convert.ToBase64String(image)), "body");
                form.Add(new StringContent(recognition_type), "target");
                form.Add(new StringContent(lang), "lang");
                form.Add(new StringContent(is_translate ? "1" : "0"), "translate");
                form.Add(new StringContent(is_qr ? "1" : "0"), "qr");

                // Send data
                var response = await _client.PostAsync(_imageURL, form);
                if (response.IsSuccessStatusCode)
                {
                    // Get json response and parse it.
                    var result = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var vbresp = JsonConvert.DeserializeObject<VBResponse>(result);
                        return vbresp;
                    }
                    catch (JsonException ex)
                    {
                        return new VBResponse() { Status = "error", Text = ex.Message };
                    } // catch
                } // if IsSuccessStatusCode
                else // Something goes wrong, return error status.
                    return new VBResponse() { Status = "error", Text = "Server don't respond." };
            } // using
        } // method

        /// <summary>
        /// Receive status of recognition process. If status ok, returns image description.
        /// </summary>
        /// <param name="id"> id of recognition process. </param>
        /// <returns>object contains status and if it ok, description and qr-code of image.</returns>
        public async Task<VBResponse> RequestStatusAsync(string id)
        {
            // Prepare data to send.
            using (var form = new MultipartFormDataContent())
            {
                form.Add(new StringContent(id), "id");

                // Send data.
                var response = await _client.PostAsync(_resultURL, form);
                if (response.IsSuccessStatusCode)
                {
                    // Parse json respond.
                    var result = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var vbresp = JsonConvert.DeserializeObject<VBResponse>(result);
                        return vbresp;
                    }
                    catch (JsonException ex)
                    {
                        return new VBResponse() { Status = "error", Text = ex.Message };
                    } //catch
                } // if IsSuccessStatusCode
                else // Something goes wrong...
                    return new VBResponse() { Status = "error", Text = "Server don't respond." };
            } //using
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
//EndFile//