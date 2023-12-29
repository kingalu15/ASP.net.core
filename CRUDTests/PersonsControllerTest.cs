using AutoFixture;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CRUDTests
{
    public class PersonsControllerTest
    {

        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countrieService;

        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;

        private readonly IFixture  _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _personsServiceMock = new Mock<IPersonsService>();
            _countriesServiceMock = new Mock<ICountriesService>();

            _personsService = _personsServiceMock.Object;
            _countrieService = _countriesServiceMock.Object;
        }

        #region Index
        [Fact]
        public async Task Index_ViewShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
             List<PersonResponse> _person_response_list = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(_personsService, _countrieService);

            _personsServiceMock.Setup(x => 
            x.GetFilteredPersons(
                It.IsAny<string>(), 
                It.IsAny<string>()))
                .ReturnsAsync(_person_response_list);

            _personsServiceMock.Setup(x =>
               x.GetSortedPersons(
                   It.IsAny<List<PersonResponse>>(),
                   It.IsAny<string>(),
                   It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(_person_response_list);

            //Act

             IActionResult result = await personsController.Index(
                    _fixture.Create<string>(),
                    _fixture.Create<string>(),
                    _fixture.Create<SortOrderOptions>().ToString()
                    );

            //Assert
              ViewResult viewResult =  Assert.IsType<ViewResult>(result);
              viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
              viewResult.ViewData.Model.Should().Be(_person_response_list);
        }


        #endregion Index


        #region Create
        [Fact]
        public async Task Create_IfModelErrors_ShouldReturnCreateView()
        {
                //Arrange
                PersonAddRequest  _person_add_request = _fixture.Create<PersonAddRequest>();
                PersonResponse _person_response = _fixture.Create<PersonResponse>();

                List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

                _countriesServiceMock.Setup(x =>
                    x.GetAllCountries())
                         .ReturnsAsync(countries);

                _personsServiceMock.Setup(x =>
                    x.AddPerson(
                         It.IsAny<PersonAddRequest>()))
                        .ReturnsAsync(_person_response);


            PersonsController personsController = new PersonsController(_personsService, _countrieService);

            //Act

            personsController.ModelState.AddModelError("PersonName", "PersonName cannot be blank");

                IActionResult result = await personsController.Create(_person_add_request);
                //Assert
                ViewResult viewResult = Assert.IsType<ViewResult>(result);

                viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
                viewResult.ViewData.Model.Should().Be(_person_add_request);
            }


        [Fact]
        public async Task Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            //Arrange
            PersonAddRequest _person_add_request = _fixture.Create<PersonAddRequest>();
            PersonResponse _person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(x =>
                x.GetAllCountries())
                     .ReturnsAsync(countries);

            _personsServiceMock.Setup(x =>
                x.AddPerson(
                     It.IsAny<PersonAddRequest>()))
                    .ReturnsAsync(_person_response);


            PersonsController personsController = new PersonsController(_personsService, _countrieService);

            //Act
            IActionResult result = await personsController.Create(_person_add_request);
            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
        }



        #endregion Create
    }
}
