using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PersonsRepository> _logger;

        public PersonsRepository(ApplicationDbContext db, ILogger<PersonsRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid personID)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonID == personID));
            int rowSelected =await  _db.SaveChangesAsync();
            return rowSelected >0;
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredPersons method of PersonsRepository");
            return await _db.Persons.Include("Country")
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<List<Person>> GetAllPerson()
        {
            _logger.LogInformation("GetAllPerson method of PersonsRepository");
            return  await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<Person?> GetPersonById(Guid personID)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == personID);
       
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);
            if(matchingPerson != null) {
                return person;
            }
            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Gender = person.Gender;
            matchingPerson.Address = person.Address;
            matchingPerson.TIN = person.TIN;
            matchingPerson.CountryID = person.CountryID;
            matchingPerson.DateOfBirth = person.DateOfBirth;
            matchingPerson.Email = person.Email;
            matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;
            await _db.SaveChangesAsync();

            return matchingPerson;  

        }
    }
}
