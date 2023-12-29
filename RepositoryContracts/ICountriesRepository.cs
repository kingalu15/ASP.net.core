using Entities;


namespace RepositoryContracts
{
    /// <summary>
    /// Represent Data access logic for managing Counry Entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Add new country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Returns country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// List all countries from table
        /// </summary>
        /// <
        /// returns></returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Return Country object based on given country id; otherwise it will returns null
        /// </summary>
        /// <param name="countryId">CountryId to search</param>
        /// <returns>Matching Country or Null</returns>

        Task<Country> GetCountryById(Guid countryId);
        /// <summary>
        /// Return Country object based on given country name, otherwise it will return null
        /// </summary>
        /// <param name="name">Country name to search</param>
        /// <returns>Matching country or Null</returns>
        Task<Country> GetCountryByName(string name);

        /// <summary>
        /// Delete Country object based on given country name
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        Task<bool> DeleteCountry(Guid countryId);
    }
}
