using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CRUDTests
{
    public class PersonControllerIntergrationTest:IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;  
        
        public PersonControllerIntergrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
        
        [Fact]
        public async Task Index_ToReturnView()
        {

            //Arrange

            //Act

            HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

            //Assert
            response.Should().BeSuccessful();
            string responseBody =  await response.Content.ReadAsStringAsync();
            HtmlDocument html = new HtmlDocument();

            html.LoadHtml(responseBody);
            var document = html.DocumentNode;
            document.QuerySelectorAll("table.persons").Should().NotBeNull();


        }
    }
}
