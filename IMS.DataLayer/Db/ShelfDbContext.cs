using System;
using System.Collections.Generic;
using System.Text;
using IMS.Entities;
using MySql.Data.MySqlClient;
using System.Data;
using IMS.DataLayer.Interfaces;
using System.Threading.Tasks;
using System.Data.Common;

namespace IMS.DataLayer
{
    public class ShelfDbContext : IShelfDbContext
    {
        private IDbConnectionProvider _dbProvider;
        public ShelfDbContext(IDbConnectionProvider dbConnectionProvider)
        {
            _dbProvider = dbConnectionProvider;
        }
        public async Task<List<Shelf>> AddShelf(Shelf shelf)
        {
            List<Shelf> Shelves = null;
            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddShelf";

                    command.Parameters.AddWithValue("@Id", shelf.Id);
                    command.Parameters.AddWithValue("@Name", shelf.Name);
                    command.Parameters.AddWithValue("@IsActive", true);
                    command.Parameters.AddWithValue("@Code", shelf.Code);
                    await command.ExecuteNonQueryAsync();
                    Shelves = await GetAllShelves();
                }
                catch (Exception e)
                {
                    throw e;
                }


            }
            return Shelves;
        }

        public async Task< List<Shelf>> DeleteShelfByCode(string shelfCode)
        {
            List<Shelf> Shelves = null;
        
            using (var connection =await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spDeleteShelfByCode";
            
                    command.Parameters.AddWithValue("@Code", shelfCode);
                  
                    await command.ExecuteNonQueryAsync();
                    Shelves =await  GetAllShelves();
                }
                catch (Exception e)
                {
                    throw e;
                }


            }
            return Shelves;
        }

        public async Task<List<Shelf>> GetAllShelves()
        {
            DbDataReader reader = null;
            List<Shelf> shelves = new List<Shelf>();

            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetAllShelves";
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        shelves.Add(new Shelf()
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"]?.ToString(),
                            IsActive = (bool)reader["IsActive"],
                            Code = reader["Code"]?.ToString()

                        });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return shelves;
        }

        public async Task<Shelf> GetShelfByShelfCode(string Code)
        {
            Shelf shelf = null;
            try
            {
                List<Shelf> _shelves = await GetAllShelves();
                _shelves.ForEach((myShelfe) => {
                    if (myShelfe.Code == Code)
                        shelf = myShelfe;
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return shelf;
        }

        public async Task<bool> GetShelfStatusByCode(string shelfCode)
        {
           
            bool shelfStatus=false;
            try
            {
                List<Shelf> _shelves =await GetAllShelves();
                _shelves.ForEach((myShelfe) => {
                    if (myShelfe.Code == shelfCode)
                        shelfStatus= myShelfe.IsActive;
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return shelfStatus;
        }

       

        public async Task<bool> IsShelfPresentByCode(string shelfCode)
        {
            
            bool isShelfPresentByCode=false;
                try
                {
                    List<Shelf> _shelves =await GetAllShelves();
                    _shelves.ForEach((myShelfe)=> {
                    if (myShelfe.Code == shelfCode)
                        isShelfPresentByCode = true;
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            
            return isShelfPresentByCode;
        }
        
        public async Task<Shelf> UpdateShelf(Shelf updatedShelf)
        {
            DbDataReader reader = null;
            Shelf shelf = new Shelf();
            using (var connection = await _dbProvider.GetConnection(Databases.IMS))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spUpdateShelf";
                    command.Parameters.AddWithValue("@id", updatedShelf.Id);
                    command.Parameters.AddWithValue("@name", updatedShelf.Name);
                    command.Parameters.AddWithValue("@isActive", updatedShelf.IsActive);
                    command.Parameters.AddWithValue("@code", updatedShelf.Code);
                    reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        shelf = ReadValuesFromDatabase(reader);
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return shelf;
            }
        }

        private Shelf ReadValuesFromDatabase(DbDataReader reader)
        {
            Shelf shelf = new Shelf();
            shelf.Id = (int)reader["Id"];
            shelf.Name = reader["Name"]?.ToString();
            shelf.IsActive = (bool)reader["IsActive"];
            shelf.Code = reader["Code"]?.ToString();
            return shelf;
        }
    }
}
