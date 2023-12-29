using System;
using System.Collections.Generic;
using Xunit;
using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using FluentAssertions.Execution;
using System.Linq.Expressions;
using AutoFixture.Kernel;
using Serilog;

namespace CRUDTests
{
  public class PersonsServiceTest
  {
    //private fields
    private readonly IPersonsService _personService;
    private readonly ICountriesService _countriesService;
    private readonly Mock<IPersonsRepository> _personRepositoryMock;
    private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
    private readonly IPersonsRepository _personsRepository; 
    private readonly ICountriesRepository _countriesRepository; 
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;   

    //constructor
    public PersonsServiceTest(ITestOutputHelper testOutputHelper)         
    {
    
            _fixture = new Fixture();
            _testOutputHelper = testOutputHelper;

            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;

            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesService = new CountriesService(_countriesRepository);
            var MockDiagnosticContext =new Mock<IDiagnosticContext>();
            _personService = new PersonsService(_personsRepository, MockDiagnosticContext.Object);
        }

    #region AddPerson

    //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullException()
    {
      //Arrange
      PersonAddRequest? personAddRequest = null;

            //Act
            Func<Task> action = async () =>
            {
              await _personService.AddPerson(personAddRequest);

            };
          await action.Should().ThrowAsync<ArgumentNullException>();
    }


    //When we supply null value as PersonName, it should throw ArgumentException
    [Fact]
    public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
    {
        //Arrange
        //PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                        .With(temp => temp.PersonName, null as string).Create();

            Person person = personAddRequest.ToPerson();

            //When PersonsRepository.AddPerson is called,its has to return the same "person" object
            _personRepositoryMock.Setup(temp=>temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person); 
            //Act
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };
            action.Should().ThrowAsync<ArgumentException>();
    }

    //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
    {
            //Arrange
            //PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = "Person name...", Email = "person@example.com", Address = "sample address", CountryID = Guid.NewGuid(), Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("2000-01-01"), ReceiveNewsLetters = true };
            //PersonAddRequest ? personAddRequest = _fixture.Create<PersonAddRequest>();
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp=>temp.Email,"someone@test.com").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            //if we supply any argument value tı the AddPerson method it should return the same
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            //Act
            PersonResponse person_response_from_add =await _personService.AddPerson(personAddRequest);

            person_response_expected.PersonID = person_response_from_add.PersonID;


            //Assert
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);


            // Assert.True(person_response_from_add.PersonID != Guid.Empty);

            // Assert.Contains(person_response_from_add, persons_list);
    }

    #endregion


    #region GetPersonByPersonID

    //If we supply null as PersonID, it should return null as PersonResponse
    [Fact]
    public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
    {
      //Arrange
      Guid? personID = null;

      //Act
      PersonResponse? person_response_from_get =await _personService.GetPersonByPersonID(personID);
     
      //Assert
      person_response_from_get.Should().BeNull();
     
     // Assert.Null(person_response_from_get);
    }


    //If we supply a valid person id, it should return the valid person details as PersonResponse object
    [Fact]
    public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
    {
            //Arange
            //CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Canada" };
            //CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            //CountryResponse country_response =await _countriesService.AddCountry(country_request);

            //PersonAddRequest person_request = new PersonAddRequest() { PersonName = "person name...", Email = "email@sample.com", Address = "address", CountryID = country_response.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Email, "someone@test.com").Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp=>temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
            //Act
            PersonResponse? person_response_from_get =await _personService.GetPersonByPersonID(person.PersonID);

            //Assert
            ///Assert.Equal(person_response_from_add, person_response_from_get);
            person_response_from_get.Should().Be(person_response_expected);
    }

    #endregion


    #region GetAllPersons

    //The GetAllPersons() should return an empty list by default
    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
      //Arrange
      var persons =  new List<Person>();
      _personRepositoryMock.Setup(temp => temp.GetAllPerson()).ReturnsAsync(persons);
      //Act
      List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

      //Assert
      //Assert.Empty(persons_from_get);
      persons_from_get.Should().BeEmpty();
    }


    //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
    [Fact]
    public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
    {
            //Arrange
            //CountryAddRequest country_request_1= _fixture.Create<CountryAddRequest>();
            //CountryAddRequest country_request_2= _fixture.Create<CountryAddRequest>();
            //CountryAddRequest country_request_3= _fixture.Create<CountryAddRequest>();

            //CountryResponse country_response_1 =await _countriesService.AddCountry(country_request_1);
            //CountryResponse country_response_2 =await _countriesService.AddCountry(country_request_2);
            //CountryResponse country_response_3 =await _countriesService.AddCountry(country_request_3);
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp=>temp.Email, "smith@text.com")
                .With(temp=>temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp=>temp.Email, "smith@text.com")
                 .With(temp=>temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "mary@text.com")
                .With(temp=>temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "rahman@text.com")
                .With(temp=>temp.Country, null as Country)
                .Create()
            };
            _personRepositoryMock.Setup(temp => temp.GetAllPerson()).ReturnsAsync(persons);

            //PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            //PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

            //PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

            //List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> response_person_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();//
                                                                                                                          //new List<PersonResponse>();
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in response_person_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
           // List<PersonResponse> person_response_list_
      //Act
      List<PersonResponse> response_person_list_actual = await _personService.GetAllPersons();

      //print persons_list_from_get
      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in response_person_list_actual)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

            //Assert
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            //{
            //  Assert.Contains(person_response_from_add, persons_list_from_get);

            //}
            response_person_list_actual.Should().BeEquivalentTo(response_person_list_expected);

    }
    #endregion


    #region GetFilteredPersons

    //If the search text is empty and search by is "PersonName", it should return all persons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText()
    {
            //Arrange
            //CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            //CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

            //CountryResponse country_response_1 =await _countriesService.AddCountry(country_request_1);
            //CountryResponse country_response_2 =await _countriesService.AddCountry(country_request_2);

            //PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            //PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

            //PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

            //List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            //List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp=>temp.Email, "smith@text.com")
                .With(temp=>temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp=>temp.Email, "smith@text.com")
                 .With(temp=>temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "mary@text.com")
                .With(temp=>temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "rahman@text.com")
                .With(temp=>temp.Country, null as Country)
                .Create()
            };
            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();
            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person,bool>>>())).ReturnsAsync(persons);


      //Act
      List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

      //print persons_list_from_get
      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_search)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

      ////Assert
      //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
      //{
      //  Assert.Contains(person_response_from_add, persons_list_from_search);
      //}
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);

    }


    //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
      //Arrange
      CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
      CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

      CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
      CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

      PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

      PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

      PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

      List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

      List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

      foreach (PersonAddRequest person_request in person_requests)
      {
        PersonResponse person_response = await _personService.AddPerson(person_request);
        person_response_list_from_add.Add(person_response);
      }

      //print person_response_list_from_add
      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_add in person_response_list_from_add)
      {
        _testOutputHelper.WriteLine(person_response_from_add.ToString());
      }

      //Act
      List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

      //print persons_list_from_get
      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_search)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

      //Assert
      //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
      //{
      //  if (person_response_from_add.PersonName != null)
      //  {
      //    if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
      //    {
      //      Assert.Contains(person_response_from_add, persons_list_from_search);
      //    }
      //  }
      //}

            persons_list_from_search.Should().OnlyContain(temp=>temp.PersonName.Contains("ma",StringComparison.OrdinalIgnoreCase));
    }

    #endregion


    #region GetSortedPersons

    //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
    [Fact]
    public async Task GetSortedPersons_ToBeSuccessful()
    {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp=>temp.Email, "smith@text.com")
                .With(temp=>temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp=>temp.Email, "smith@text.com")
                 .With(temp=>temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "mary@text.com")
                .With(temp=>temp.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "rahman@text.com")
                .With(temp=>temp.Country, null as Country)
                .Create()
            };
            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();
            _personRepositoryMock.Setup(x => x.GetAllPerson()).ReturnsAsync(persons);
   
      List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
      //print person_response_list_from_add
      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_add in person_response_list_expected)
      {
        _testOutputHelper.WriteLine(person_response_from_add.ToString());
      }
      List<PersonResponse> allPersons = await  _personService.GetAllPersons();

      //Act
      List<PersonResponse> persons_list_from_sort = await  _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

      //print persons_list_from_get
      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_sort)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }
      person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            //for (int i = 0; i < person_response_list_from_add.Count; i++)
            //{
            //  Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
            //}
            //      persons_list_from_sort.Should().BeEquivalentTo(person_response_list_from_add);
            persons_list_from_sort.Should().BeInDescendingOrder(temp=>temp.PersonName);
        }
    #endregion


    #region UpdatePerson

    //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_NullPerson_ToBeArgumentNUllException()
    {
      //Arrange
      PersonUpdateRequest? person_update_request = null;
      //act
      //person_update_request = person_
      Func<Task> action = async () => {
         await  _personService.UpdatePerson(person_update_request);
      };
      //Assert
      await action.Should().ThrowAsync<ArgumentNullException>();
    }


    //When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
    {
      //Arrange
      PersonUpdateRequest? person_update_request = new PersonUpdateRequest() {  PersonID = Guid.NewGuid() };

            //act
            Func<Task> action = async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            };
            //Assert
           await action.Should().ThrowAsync<ArgumentException>();

    }


    //When PersonName is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_PersonNameIsNullAsync_ToBeArgumentException()
    {
            //Arrange
            Person person = _fixture.Build<Person>()
                   .With(temp => temp.PersonName, null as string)
                   .With(temp => temp.Email, "smith@text.com")
                   .With(x => x.Gender, "Male")
                   .With(temp => temp.Country, null as Country)
                   .Create();
              PersonResponse person_response_from_add = person.ToPersonResponse();
              PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
    
            //act
       Func<Task> action = async () =>
       {
           await _personService.UpdatePerson(person_update_request);
       };
       //Assert
       await action.Should().ThrowAsync<ArgumentException>();
    }


    //First, add a new person and try to update the person name and email
    [Fact]
    public async Task UpdatePerson_PersonFullDetailsUpdation()
    {
      //Arrange


      Person person = _fixture.Build<Person>()
                .With(x=>x.Email, "someone@example.com")
                .With(x=>x.Country, null as Country)
                .With(x => x.Gender,"Male")
                .Create();

      PersonResponse person_response_expected = person.ToPersonResponse();

      PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();
            _personRepositoryMock.Setup(x => x.UpdatePerson(It.IsAny<Person>()))
                      .ReturnsAsync(person);

            _personRepositoryMock.Setup(x => x.GetPersonById(It.IsAny<Guid>()))
               .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await  _personService.UpdatePerson(person_update_request);
            //Assert
            person_response_from_update.Should().Be(person_response_expected);
  
    }

    #endregion


    #region DeletePerson

    //If you supply an valid PersonID, it should return true
    [Fact]
    public async Task DeletePerson_ValidPersonID()
    {
            //Arrange
            Person person = _fixture.Build<Person>()
                 .With(x => x.Email, "someone@example.com")
                 .With(x => x.Country, null as Country)
                 .With(x => x.Gender, "Male")
                 .Create();

      PersonResponse person_response_from_add = person.ToPersonResponse();
            _personRepositoryMock.Setup(x => x.DeletePersonByPersonId(It.IsAny<Guid>()))
              .ReturnsAsync(true);

            _personRepositoryMock.Setup(x => x.GetPersonById(It.IsAny<Guid>()))
               .ReturnsAsync(person);


            //Act
            bool isDeleted = await _personService.DeletePerson(person_response_from_add.PersonID);

            //Assert
            isDeleted.Should().BeTrue();
    }


    //If you supply an invalid PersonID, it should return false
    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
      //Act
      bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //Assert
            isDeleted.Should().BeFalse();

    }

    #endregion
  }
}
