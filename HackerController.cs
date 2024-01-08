using HackerApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Mime;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HackerApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class HackerController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly IMemoryCache _memoryCache;

        // GET: api/<HackerController>

        public HackerController(HttpClient client,IMemoryCache memoryCache)
        {
            this._client = client;
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

        }

        [HttpGet]
        public async Task<IEnumerable<Dictionary<string,string>>> GetStories()
        {
            try
            {
                const string cacheKey = "dfdghfghfgdfeertrhg";
                if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Dictionary<string, string>> cachedData))
                {
                    // Use cached data if available
                    return cachedData;
                }

                string baseUri1 = "https://hacker-news.firebaseio.com/v0/item/";
                string baseUri2 = ".json?print=pretty";
                string storiesUrl = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";
                //string storiesUrl = "https://hacker-news.firebaseio.com/v0/item/38901012.json?print=pretty";
                HttpResponseMessage response = await _client.GetAsync(storiesUrl);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    int[]? allids = JsonSerializer.Deserialize<int[]>(content);
                    int[] ids = allids.Take(200).ToArray();
                    //int[] ids = allids.Take(15).ToArray();
                    //List<Story> stories = new List<Story>();
                    List<Dictionary<string,string>> stories = new List<Dictionary<string,string>>();
                    StreamWriter sw = new StreamWriter("D:\\r system\\log.txt", true);
                    foreach (var item in ids)
                    {
                        string finalUrl = baseUri1 + item+baseUri2;
                        
                        sw.WriteLine(finalUrl);
                       HttpResponseMessage response2 = await _client.GetAsync(baseUri1 + item + baseUri2);
                        if (response2.IsSuccessStatusCode) 
                        {
                            string content2 = await response2.Content.ReadAsStringAsync();
                            var tempstory= JsonDocument.Parse(content2);
                            //["url"]= tempstory.RootElement.GetProperty("url").GetString()
                            var filteredstory =new Dictionary<string, string>
                            {
                                ["title"]= GetPropertyOrNull(tempstory.RootElement,"title"),
                                ["url"] = GetPropertyOrNull(tempstory.RootElement, "url")
                            };

                            stories.Add(filteredstory);
                            //var tempstory = JsonSerializer.Deserialize<Story>(content2);

                            // stories.Add(new Story { title=tempstory.title, url = tempstory.url });

                            //IEnumerable<dynamic>? dynamicList = JsonSerializer.Deserialize<IEnumerable<dynamic>>(content2);
                            //List<Story> stories= dynamicList.Select(item => new Story {Title=item.title, Url=item.url}).ToList();
                            var cacheEntryOptions = new MemoryCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                            };

                            // Store data in cache
                            _memoryCache.Set(cacheKey, stories, cacheEntryOptions);

                        }
                        else
                        {
                            Response.StatusCode = (int)response2.StatusCode;
                            return null;
                            //return (IEnumerable<Dictionary<string,string>>)StatusCode((int)response2.StatusCode, "Failed to Retrieve data");
                        }
                       
                    }
                    return stories;
                }
                else
                {
                    Response.StatusCode = (int)response.StatusCode;
                    return null;
                    //return (IEnumerable<Dictionary<string, string>>)StatusCode((int)response.StatusCode, "Failed to Retrieve data");
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return null;
                //return (IEnumerable<Dictionary<string, string>>)StatusCode(500, $"An error occurred: {ex.Message}");

            }
        }

        private string GetPropertyOrNull(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var property))
            {
                return property.GetString();
            }

            return null;
        }


        // GET api/<HackerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<HackerController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<HackerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HackerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
