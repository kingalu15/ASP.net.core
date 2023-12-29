﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options) :base(options)
        {

        }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
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

            //Fluent API
            modelBuilder.Entity<Person>().Property(temp => temp.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");

            // modelBuilder.Entity<Person>().HasIndex(temp => temp.TIN).IsUnique;
            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN_LEN", "len(TaxIdentificationNumber)=8");

            //Table Relations
            //modelBuilder.Entity<Person>(entity =>
            //{
            //    entity.HasOne<Country>(c => c.Country).WithMany(p=>p.Persons)
            //    .HasForeignKey(p=>p.CountryID);
            //});
        }
       
        public List<Person> sp_GetAllPersons()
        {
          return  Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }
        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID",person.PersonID),
                new SqlParameter("@PersonName",person.PersonName),
                new SqlParameter("@Email",person.Email),
                new SqlParameter("@DateOfBirth",person.DateOfBirth),
                new SqlParameter("@Gender",person.Gender),
                new SqlParameter("@CountryID",person.CountryID),
                new SqlParameter("@Address",person.Address),
                new SqlParameter("@ReceiveNewsLetters",person.ReceiveNewsLetters),
            };
            int result =  Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID,@PersonName,@Email,@DateOfBirth,@Gender,@CountryID,@Address,@ReceiveNewsLetters");
            return result;
        }
    }
}
