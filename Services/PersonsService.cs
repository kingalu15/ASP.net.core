using System;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using ServiceContracts.Enums;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Serilog;
using SerilogTimings;

namespace Services
{
  public class PersonsService : IPersonsService
  {
    //private field
   // private readonly List<Person> _persons;
    //private readonly ICountriesRepository _countriesRepository;
    private readonly IPersonsRepository _personRepository;
    private readonly ILogger<PersonsService> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    //constructor
    public PersonsService(IPersonsRepository personRepository,IDiagnosticContext diagnosticContext)
        {
            _personRepository = personRepository;
            _diagnosticContext = diagnosticContext;
           // _logger = logger;


            //if (initialize)
            //{
            //  _persons.Add(new Person() { PersonID = Guid.Parse("8082ED0C-396D-4162-AD1D-29A13F929824"), PersonName = "Aguste", Email = "aleddy0@booking.com", DateOfBirth = DateTime.Parse("1993-01-02"), Gender = "Male", Address = "0858 Novick Terrace", ReceiveNewsLetters = false, CountryID = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B") });

            //  _persons.Add(new Person() { PersonID = Guid.Parse("06D15BAD-52F4-498E-B478-ACAD847ABFAA"), PersonName = "Jasmina", Email = "jsyddie1@miibeian.gov.cn", DateOfBirth = DateTime.Parse("1991-06-24"), Gender = "Female", Address = "0742 Fieldstone Lane", ReceiveNewsLetters = true, CountryID = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F") });

            //  _persons.Add(new Person() { PersonID = Guid.Parse("D3EA677A-0F5B-41EA-8FEF-EA2FC41900FD"), PersonName = "Kendall", Email = "khaquard2@arstechnica.com", DateOfBirth = DateTime.Parse("1993-08-13"), Gender = "Male", Address = "7050 Pawling Alley", ReceiveNewsLetters = false, CountryID = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F") });

            //  _persons.Add(new Person() { PersonID = Guid.Parse("89452EDB-BF8C-4283-9BA4-8259FD4A7A76"), PersonName = "Kilian", Email = "kaizikowitz3@joomla.org", DateOfBirth = DateTime.Parse("1991-06-17"), Gender = "Male", Address = "233 Buhler Junction", ReceiveNewsLetters = true, CountryID = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E") });

            //  _persons.Add(new Person() { PersonID = Guid.Parse("F5BD5979-1DC1-432C-B1F1-DB5BCCB0E56D"), PersonName = "Dulcinea", Email = "dbus4@pbs.org", DateOfBirth = DateTime.Parse("1996-09-02"), Gender = "Female", Address = "56 Sundown Point", ReceiveNewsLetters = false, CountryID = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E") });

            //  _persons.Add(new Person() { PersonID = Guid.Parse("A795E22D-FAED-42F0-B134-F3B89B8683E5"), PersonName = "Corabelle", Email = "cadams5@t-online.de", DateOfBirth = DateTime.Parse("1993-10-23"), Gender = "Female", Address = "4489 Hazelcrest Place", ReceiveNewsLetters = false, CountryID = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D") });

            //  _persons.Add(new Person() { PersonID = Guid.Parse("3C12D8E8-3C1C-4F57-B6A4-C8CAAC893D7A"), PersonName = "Faydra", Email = "fbischof6@boston.com", DateOfBirth = DateTime.Parse("1996-02-14"), Gender = "Female", Address = "2010 Farragut Pass", ReceiveNewsLetters = true, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

            //  _persons.Add(new Person() { PersonID = Guid.Parse("7B75097B-BFF2-459F-8EA8-63742BBD7AFB"), PersonName = "Oby", Email = "oclutheram7@foxnews.com", DateOfBirth = DateTime.Parse("1992-05-31"), Gender = "Male", Address = "2 Fallview Plaza", ReceiveNewsLetters = false, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

            //  _persons.Add(new Person() { PersonID = Guid.Parse("6717C42D-16EC-4F15-80D8-4C7413E250CB"), PersonName = "Seumas", Email = "ssimonitto8@biglobe.ne.jp", DateOfBirth = DateTime.Parse("1999-02-02"), Gender = "Male", Address = "76779 Norway Maple Crossing", ReceiveNewsLetters = false, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

            //  _persons.Add(new Person() { PersonID = Guid.Parse("6E789C86-C8A6-4F18-821C-2ABDB2E95982"), PersonName = "Freemon", Email = "faugustin9@vimeo.com", DateOfBirth = DateTime.Parse("1996-04-27"), Gender = "Male", Address = "8754 Becker Street", ReceiveNewsLetters = false, CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB") });

            //}
    }


    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
      //check if PersonAddRequest is not null
      if (personAddRequest == null)
      {
        throw new ArgumentNullException(nameof(personAddRequest));
      }

      //Model validation
      ValidationHelper.ModelValidation(personAddRequest);

      //convert personAddRequest into Person type
      Person person = personAddRequest.ToPerson();

      //generate PersonID
      person.PersonID = Guid.NewGuid();

      //add person object to persons list
      await _personRepository.AddPerson(person);
      //convert the Person object into PersonResponse type
       return person.ToPersonResponse();
    }


    public async Task<List<PersonResponse>> GetAllPersons()
    {
            _logger.LogInformation("GetAllPersons method of PersonsService");
          
            var persons = await _personRepository.GetAllPerson();
           return   persons.Select(temp => temp.ToPersonResponse()).ToList();
            //return _personRepository.Persons.ToList().Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
            //return _personRepository.sp_GetAllPersons().Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
    }


    public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
    {
      if (personID == null)
        return null;

      Person? person =await _personRepository.GetPersonById(personID.Value);
      if (person == null)
        return null;
      return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
            // _logger.LogInformation("GetFilteredPersons method of PersonsService");
            List<Person> persons;
            using (Operation.Time("Time for Filtered Persons from Database"))
            {
                persons = searchBy switch
                {
                    nameof(PersonResponse.PersonName) =>
                    await _personRepository.GetFilteredPersons(temp => temp.PersonName.Contains(searchString)),


                    nameof(PersonResponse.Email) =>
                    await _personRepository.GetFilteredPersons(temp => temp.Email.Contains(searchString)),

                    nameof(PersonResponse.DateOfBirth) =>
                    await _personRepository.GetFilteredPersons(temp => temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),

                    nameof(PersonResponse.Gender) =>
                    await _personRepository.GetFilteredPersons(temp => temp.Gender.Contains(searchString)),

                    nameof(PersonResponse.CountryID) =>
                    await _personRepository.GetFilteredPersons(temp => temp.Country.CountryName.Contains(searchString)),

                    nameof(PersonResponse.Address) =>
                    await _personRepository.GetFilteredPersons(temp => temp.Address.Contains(searchString)),

                    _ => await _personRepository.GetAllPerson()

                };
            }
            _diagnosticContext.Set("Persons", persons);
            return persons.Select(temp => temp.ToPersonResponse()).ToList();

    }


    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
    {
           // _logger.LogInformation("GetSortedPersons method of PersonsService");
            if (string.IsNullOrEmpty(sortBy))
        return allPersons;

      List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
      {
        (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

        (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

        (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

        (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

        (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

        (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

        (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

        _ => allPersons
      };

      return sortedPersons;
    }


    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
      if (personUpdateRequest == null)
        throw new ArgumentNullException(nameof(Person));

      //validation
      ValidationHelper.ModelValidation(personUpdateRequest);

      //get matching person object to update
      Person? matchingPerson =await _personRepository.GetPersonById(personUpdateRequest.PersonID);
      if (matchingPerson == null)
      {
        throw new ArgumentException("Given person id doesn't exist");
      }
      await _personRepository.UpdatePerson(matchingPerson);
      return matchingPerson.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personID)
    {
      if (personID == null)
      {
        throw new ArgumentNullException(nameof(personID));
      }

      Person? person = await _personRepository.GetPersonById(personID.Value);
      if (person == null)
        return false;

      return await _personRepository.DeletePersonByPersonId(personID.Value);
    }

        /// <summary>
        /// Return Persons as CSV
        /// </summary>
        /// <returns>Return memory stream as csv</returns>
     public async Task<MemoryStream> GetPersonsCSV()
     {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWrite = new CsvWriter(streamWriter, csvConfiguration);

            //Write property name as header
            //csvWrite.WriteHeader<PersonResponse>();
            csvWrite.WriteField(nameof(PersonResponse.PersonName));
            csvWrite.WriteField(nameof(PersonResponse.Email));
            csvWrite.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWrite.WriteField(nameof(PersonResponse.Age));
            csvWrite.WriteField(nameof(PersonResponse.Gender));
            csvWrite.WriteField(nameof(PersonResponse.Country));
            csvWrite.WriteField(nameof(PersonResponse.Address));
            csvWrite.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
            csvWrite.NextRecord();

            List<PersonResponse> persons = await GetAllPersons();

            foreach(PersonResponse person in persons)
            {
                csvWrite.WriteField(person.PersonName);
                csvWrite.WriteField(person.Email);
                if (person.DateOfBirth.HasValue) csvWrite.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                else csvWrite.WriteField("");
                csvWrite.WriteField(person.Age);
                csvWrite.WriteField(person.Gender);
                csvWrite.WriteField(person.Country);
                csvWrite.WriteField(person.Address);
                csvWrite.WriteField(person.ReceiveNewsLetters);
                csvWrite.NextRecord();
                csvWrite.Flush();

            }
           // await csvWrite.WriteRecordsAsync(persons);
            memoryStream.Position = 0;
            return memoryStream;
        }  
        
     /// <summary>
     /// Return Persons as CSV
     /// </summary>
     /// <returns>Return memory stream as csv</returns>
     public async Task<MemoryStream> GetPersonsExcel()
     {
            MemoryStream memoryStream = new MemoryStream();
            using(ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                workSheet.Cells["A1"].Value = "Person Name";              
                workSheet.Cells["B1"].Value = "Email";         
                workSheet.Cells["C1"].Value = "Date Of Birth";         
                workSheet.Cells["D1"].Value = "Age";         
                workSheet.Cells["E1"].Value = "Gender";         
                workSheet.Cells["F1"].Value = "Country";         
                workSheet.Cells["G1"].Value = "Address";         
                workSheet.Cells["H1"].Value = "ReceiveNewsLetters";

                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();

                foreach (PersonResponse person in persons)
                {
                    workSheet.Cells[row,1].Value = (person.PersonName);
                    workSheet.Cells[row,2].Value =(person.Email);
                    if (person.DateOfBirth.HasValue) workSheet.Cells[row, 3].Value = (person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                    else workSheet.Cells[row, 3].Value = ("");
                    workSheet.Cells[row, 4].Value =(person.Age);
                    workSheet.Cells[row, 5].Value =(person.Gender);
                    workSheet.Cells[row, 6].Value =(person.Country);
                    workSheet.Cells[row, 7].Value =(person.Address);
                    workSheet.Cells[row, 8].Value =(person.ReceiveNewsLetters);
                    row++;
                }
                workSheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await excelPackage.SaveAsync(); 
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
  }
}
