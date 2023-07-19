using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entities
{
    public class PersonsDbContext:DbContext
    {
        public PersonsDbContext(DbContextOptions options) :base(options)
        {

        }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; }
       protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to countries
            string countriesJson = System.IO.File.ReadAllText("Countries.json");
            List<Country> countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach(Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(new Country()
                {
                    CountryID = country.CountryID,
                    CountryName = country.CountryName,
                });

            }

            //Seed to persons
            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person> persons = JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (Person person in persons)
            {
                modelBuilder.Entity<Person>().HasData(new Person()
                {
                    PersonID = person.PersonID,
                    CountryID = person.CountryID,
                    PersonName = person.PersonName,
                    DateOfBirth = person.DateOfBirth,
                    Gender = person.Gender,
                    ReceiveNewsLetters = person.ReceiveNewsLetters,
                    Email = person.Email,
                    Address= person.Address,
                });

            }


        }
    }
}
