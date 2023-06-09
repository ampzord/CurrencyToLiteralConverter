using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CurrencyNumberConverterClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SendRequestButton(object sender, RoutedEventArgs e)
        {
            var client = CreateHttpClient();

            string inputCurrency = ((ComboBoxItem)InputCurrencyValue.SelectedItem).Name;
            string inputValue = InputNumberValue.Text;

            try
            {
                HttpResponseMessage response = await client.GetAsync(
                        $"api/numberconverter?value={inputValue}&currency={inputCurrency}");
                if (response.IsSuccessStatusCode)
                {
                    var responseResult = await response.Content.ReadAsStringAsync();

                    JToken jt = JToken.Parse(responseResult);
                    var deserializeResponse = JsonConvert.DeserializeObject<ConvertNumberResponse>(jt.ToString());
                    ResultOutput.Text = deserializeResponse.result;
                }
                else
                {
                    ResultOutput.Text = string.Empty;
                    var responseResult = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Request Error - " + response.ReasonPhrase + "\n" + responseResult);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Something went wrong!");
            }
            
        }

        record ConvertNumberResponse(string result);

        private HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7251/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

    }
}