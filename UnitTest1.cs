using RestSharp;

namespace UnitTest
{
    public class Tests
    {

        private const string BaseUrl = "http://localhost:5101/api/hacker";
        private const string Endpoint = "/GetStories";


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Endpoint, Method.Get);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

        }
    }
}