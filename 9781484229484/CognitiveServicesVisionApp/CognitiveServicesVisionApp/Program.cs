using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CognitiveServicesVisionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string> resultDescribe = DescribeImage(@"C:\Users\anurag\Downloads\photo.jpg");
            Task<string> resultText = ExtractText(@"C:\Users\anurag\Downloads\another-photo.jpg");
            Console.WriteLine(resultDescribe.Result);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(resultText.Result);
        }

        public static async Task<string> DescribeImage(string imageFilePath)
        {
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["AzureSubscriptionKeyVision"]);

                using (MultipartFormDataContent reqContent = new MultipartFormDataContent())
                {
                    var queryString = HttpUtility.ParseQueryString(string.Empty);
                    queryString["maxCandidates"] = "1";
                    var uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/describe?" + queryString;

                    try
                    {
                        var imgContent = new ByteArrayContent(System.IO.File.ReadAllBytes(imageFilePath));
                        reqContent.Add(imgContent);

                        HttpResponseMessage resp = await hc.PostAsync(uri, reqContent);
                        string respJson = await resp.Content.ReadAsStringAsync();
                        return respJson;
                    }
                    catch(System.IO.FileNotFoundException ex)
                    {
                        return "The specified image file path is invalid.";
                    }
                    catch(ArgumentException ex)
                    {
                        return "The HTTP request object does not seem to be correctly formed.";
                    }
                }
            }
        }

        public static async Task<string> ExtractText(string imageFilePath)
        {
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["AzureSubscriptionKeyVision"]);

                using (MultipartFormDataContent reqContent = new MultipartFormDataContent())
                {
                    var uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr";

                    try
                    {
                        var imgContent = new ByteArrayContent(System.IO.File.ReadAllBytes(imageFilePath));
                        reqContent.Add(imgContent);

                        HttpResponseMessage resp = await hc.PostAsync(uri, reqContent);
                        string respJson = await resp.Content.ReadAsStringAsync();
                        return respJson;
                    }
                    catch (System.IO.FileNotFoundException ex)
                    {
                        return "The specified image file path is invalid.";
                    }
                    catch (ArgumentException ex)
                    {
                        return "The HTTP request object does not seem to be correctly formed.";
                    }
                }
            }
        }
    }
}
