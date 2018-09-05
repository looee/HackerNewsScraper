using ClassLibrary;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HackerNewsScraper
{
    internal class Program
    {
        public static int[] ListOfIds { get; set; }

        public static int RankIncrement { get; set; }

        private static void Main(string[] args)
        {
            //Sets the initial rank as 1
            RankIncrement = 1;
            int x;
            Console.WriteLine("Please enter the number of posts you want to see (0-100):");
            String Result = Console.ReadLine();
            //Checks to see if the input is valid, if not keeps prompting.
            while (!Int32.TryParse(Result, out x) && x > 0 && x <= 100)
            {
                Console.WriteLine("Not a valid number, try again.");

                Result = Console.ReadLine();
            }

            x = Int32.Parse(Result);
            RunAsync(x).Wait();
        }

        private static async Task RunAsync(int x)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Get a list of all the IDs that have been asked for:
                await GetTopArticleIds(client, x);

                foreach (int id in ListOfIds)
                {
                    await GetArticleInfo(client, id);
                }
            }

            Console.ReadLine();

        }
        private static async Task GetArticleInfo(HttpClient cons, int id)
        {
            try
            {
                HttpResponseMessage response = await cons.GetAsync($"{cons.BaseAddress}v0/item/{id}.json?print=pretty");
                response.EnsureSuccessStatusCode();
                RootObject jsonObject = await response.Content.ReadAsAsync<RootObject>();
                var hackerNews = new HackerNewsOutput
                {
                    author = jsonObject.by,
                    comments = jsonObject.descendants,
                    points = jsonObject.score,
                    rank = RankIncrement,
                    title = jsonObject.title,
                    uri = jsonObject.url
                };
                //Increments the rank
                RankIncrement++;
                Console.WriteLine("\n");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(hackerNews, Formatting.Indented));

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

        }


        private static async Task GetTopArticleIds(HttpClient cons, int numberOfArticles)
        {
            try
            {
                HttpResponseMessage response = await cons.GetAsync($"{cons.BaseAddress}v0/topstories.json");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                //Outputs the json into an int array so it can be used. Only gets the number wanted
                ListOfIds = responseBody.Trim('[', ']').Split(',').Select(n => Convert.ToInt32(n)).Take(numberOfArticles).ToArray();

                //writes the list for debugging
                //Console.WriteLine(ListOfIds);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

        }



    }
}
