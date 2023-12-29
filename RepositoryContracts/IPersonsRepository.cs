using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// Represent data access logic for managing person enitiy
    /// </summary>
    public interface IPersonsRepository
    {

        /// <summary>
        /// Add a person object to the data store/table
        /// </summary>
        /// <param name="person">Person object to add</param>
        /// <returns>Return the person object after adding it to the table</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Update a person object and store 
        /// </summary>
        /// <param name="person">Person object with updated information</param>
        /// <returns>Rerun the updated person object</returns>
        Task<Person> UpdatePerson(Person person);

        /// <summary>
        /// Delete a person object from the table
        /// </summary>
        /// <param name="personID">Person object Id need to be deleted</param>
        /// <returns>Rewturn true if success or false if failed</returns>
        Task<bool> DeletePersonByPersonId(Guid personID);

        /// <summary>
        /// Get all persons from data store
        /// </summary>
        /// <param></param>
        /// <returns>Return all persons object from table</returns>
        Task<List<Person>> GetAllPerson();


        /// <summary>
        /// Get a person object matched with the Person Id supplied 
        /// </summary>
        /// <param name="personID"></param>
        /// <returns>Return person object Mathced with suuplied id or Null</returns>
        Task<Person?> GetPersonById(Guid personID);

        /// <summary>
        /// Return all person based on the given expression 
        /// </summary>
        /// <param name="predicate">Linq expression to check</param>
        /// <returns>All matching persons</returns>

        Task<List<Person>> GetFilteredPersons(Expression<Func<Person,bool>> predicate);
    }
}
