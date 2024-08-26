 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class LoL
    {
        public static async void dfasdg()
        {
            string apiKey = "TU_CLAVE_API";
            string songTitle = "Shape of You";
            string apiUrl = $"https://api.genius.com/search?q={songTitle}&access_token={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonResponse);
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                }
            }
        }

    }
}
