using ClassLibrary;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HackerNewsScraper
{
    public class Program
    {
        /// <summary>
        /// A list of ids from the top 500 posts on hackernews, limited to the number specified by the user
        /// </summary>
        public static int[] ListOfIds { get; set; }

        public static int RankIncrement { get; set; }

        public  static HackerNewsOutput HackerNews { get; set; }

        public static void Main(string[] args)
        {
            //Sets the initial rank as 1
            RankIncrement = 0;
            int x;
            //Console.WriteLine("Please enter the number of posts you want to see (0-100):");
            //String Result = Console.ReadLine();
            string Result = args.First();
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
            //Creates new http client to be destroyed later
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/");                
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Get a list of all the IDs that have been asked for:
                await GetTopArticleIds(client, x);

                foreach (int id in ListOfIds)
                {
                    await GetSingleArticleInfo(client, $"v0/item/{id}.json?print=pretty");
                }
            }
            //For debugging
            //Console.ReadLine();
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

        private static async Task GetSingleArticleInfo(HttpClient cons, string subURL)
        {
            try
            {
                HttpResponseMessage response = await cons.GetAsync($"{cons.BaseAddress}{subURL}");
                response.EnsureSuccessStatusCode();
                RootObject jsonObject = await response.Content.ReadAsAsync<RootObject>();

                ValidateAndArrangeArticleObject(jsonObject);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

        }

        public static void ValidateAndArrangeArticleObject(RootObject jsonObject)
        {           

            //Checks to see if the author is empty, or it is longer than 256 characters. 
            //Checks to see if the title is empty, or it is longer than 256 characters.
            //Checks to see if uri is valid according to URI.IsWellFormedUriString().
            //Checks to see if score is greater or equal to zero
            //Checks to see if decendents/comments is greater or equal to zero
            //If there are no problems continue. Otherwise skip this article.
            if (!string.IsNullOrEmpty(jsonObject.by) && jsonObject.by.Length <= 256 && !string.IsNullOrEmpty(jsonObject.title) &&
                jsonObject.title.Length <= 256 && Uri.IsWellFormedUriString(jsonObject.url, UriKind.Absolute) && jsonObject.score >= 0 &&
                jsonObject.descendants >= 0)
            {
                //Increments the rank
                RankIncrement++;
                HackerNews = new HackerNewsOutput
                {
                    author = jsonObject.by,
                    comments = jsonObject.descendants,
                    points = jsonObject.score,
                    rank = RankIncrement,
                    title = jsonObject.title,
                    uri = jsonObject.url
                };
                Console.WriteLine("\n");
                Console.WriteLine(JsonConvert.SerializeObject(HackerNews, Formatting.Indented));
            }
            else
            {
                //clears the object so it doesn't get used again.
                HackerNews = null;
            }
        }




    }
}
