using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchStreamingApp
{
    public partial class MainPage : ContentPage
    {
        private const string ClientId = "lt3p7af6vamltum6bkp5s3qf94gjwi"; 
        private const string AccessToken = "nfkzkeyle69a2lgnji89ld303c9pds"; 

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSearchButtonClicked(object sender, EventArgs e)
        {
            //Cambia para orientar al usuario
            Boton.BackgroundColor = Color.DarkSlateGray;
            streamStatusLabel.Text = "........................";

            string streamerName = streamerNameEntry.Text?.Trim(); // Obtiene el nombre del streamer del campo de entrada
            if (string.IsNullOrWhiteSpace(streamerName))
            {
                await DisplayAlert("Error", "Por favor ingresa un nombre de streamer.", "OK");
                Boton.BackgroundColor = Color.Aqua;
                return;
            }

            // Aquí se llama a la API de Twitch para obtener información sobre el streamer
            var streamerLiveData = await GetStreamerLiveStatusAsync(streamerName);

            if (streamerLiveData != null && streamerLiveData.Data.Length > 0)
            {
                var stream = streamerLiveData.Data[0];

                streamStatusLabel.Text = $"{stream.UserName} está transmitiendo ahora!";
                streamStatusLabel.TextColor = Color.Green;
                streamStatusLabel.IsVisible = true;

                // USa la URL para buscar al streamer
                string streamUrl = $"https://www.twitch.tv/{stream.UserName}";
                streamWebView.Source = streamUrl;
                streamWebView.IsVisible = true;

                Boton.BackgroundColor = Color.Aqua;

                Boton2.IsVisible = true;

            }
            else
            {
                //Si el streamer no esta en directo o no existe
                streamStatusLabel.Text = "El streamer está offline o no existe.";
                streamStatusLabel.TextColor = Color.Red;
                streamStatusLabel.IsVisible = true;
                streamWebView.IsVisible = false;

                Boton.BackgroundColor = Color.Aqua;

                Boton2.IsVisible = false;

            }
        }


        private async void CloseWebViewClicked(object sender, EventArgs e)
        {
            streamWebView.IsVisible = false;
            Boton2.IsVisible=false;

            streamStatusLabel.Text = "";
        }



        // Método para obtener el estado en vivo del streamer
        private async Task<StreamResponse> GetStreamerLiveStatusAsync(string streamerName)
        {
            string url = $"https://api.twitch.tv/helix/streams?user_login={streamerName}";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
                httpClient.DefaultRequestHeaders.Add("Client-ID", ClientId);

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<StreamResponse>(jsonResponse);
                }
                else
                {
                    await DisplayAlert("Error", "Error al verificar el estado del streamer. Intenta de nuevo.", "OK");
                    return null;
                }
            }
        }
    }

    // Clases para convertir y usar la respuesta de la API de Twitch
    public class StreamResponse
    {
        public StreamData[] Data { get; set; }
    }

    public class StreamData
    {
        [JsonProperty("user_name")]
        public string UserName { get; set; }
    }
}
