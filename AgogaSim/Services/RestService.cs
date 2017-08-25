using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgogaSim
{
    public class RestService
    {
        Uri baseUri;
        HttpClient client;

        public RestService()
        {
            baseUri = new Uri("https://www.ahgora.com.br/externo/");

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Accept.Clear();
        }

        Report loadReport(string content)
        {
            var report = JsonConvert.DeserializeObject<Report>(content);

            JObject agogaData = JObject.Parse(content);
            IList<JToken> daysTokens = agogaData["dias"].Children().ToList();
            IList<DayReport> days = new List<DayReport>();
            foreach(JToken dayValue in daysTokens)
            {
                JToken dayContent = dayValue.Children().First();
                DayReport day = dayContent.ToObject<DayReport>();

                JProperty dayAsProperty = (JProperty)dayValue;
                if (dayAsProperty != null)
                    day.SetDay(dayAsProperty.Name);
                
                days.Add(day);
			}

            report.Days = days;
            return report;
        }

        public async Task<Report> LoadData(string company, string id, string password, DateTime date)
        {
            var uri = new Uri(baseUri, "getApuracao");

            try
            {
                var data = new Dictionary<string, string> {
                    { "company", company },
                    { "matricula", id },
                    { "senha", password },
                    { "mes", date.Month.ToString().PadLeft(2, '0') },
                    { "ano", date.Year.ToString() }
                };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(data));
                jsonContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                var response = await client.PostAsync(uri, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(content);
                    return loadReport(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"ERROR {0}", ex.Message);
                return null;
            }

            return null;
        }
    }
}
