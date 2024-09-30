


using System.Data;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OrderServiceQuery.Infrastructure.DatabaseContext;
using OrderServiceQuery.Core.Repositories;

namespace OrderServiceQuery.Infrastructure.UnitOfWork
{
    public class UnitOfWork<ReposSide> : IUnitOfWork<ReposSide> where ReposSide : Side
    {
        private readonly AppDbContext<ReposSide> _context;

        public UnitOfWork(AppDbContext<ReposSide> context)
        {
            this._context = context;
        }

        public void ExecuteCommand(string sql)
        {
            this._context.Database.ExecuteSqlRaw(sql);
        }

        public async Task<IEnumerable<T>> Query<T>(string query, CommandType commandType = CommandType.Text, object? parameters = null, string? connectionString = null, IDictionary<string, object?>? outputParameters = null)
        {
            if(connectionString == null)
            {
                connectionString = this._context.Database.GetDbConnection().ConnectionString;
            }

            return await Query<T>(connectionString, query, parameters, commandType, outputParameters);
        }

        private async Task<IEnumerable<T>> Query<T>(string connectionString, string query, object? parameters = null, CommandType commandType = CommandType.Text, IDictionary<string, object?>? outputParameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = commandType;

                    if (parameters != null)
                    {
                        foreach (PropertyInfo prop in parameters.GetType().GetProperties())
                        {
                            var param = command.CreateParameter();
                            param.ParameterName = prop.Name;
                            param.Value = prop.GetValue(parameters) ?? DBNull.Value;
                            command.Parameters.Add(param);
                        }
                    }

                    if (outputParameters != null)
                    {
                        foreach (var dic in outputParameters)
                        {
                            var param = command.CreateParameter();
                            param.ParameterName = dic.Key;
                            param.SqlDbType = SqlDbType.NVarChar;
                            param.Direction = ParameterDirection.Output;
                            param.Value = dic.Value ?? DBNull.Value;
                            param.Size = 1000;

                            command.Parameters.Add(param);
                        }
                    }

                    await connection.OpenAsync();

                    var list = new List<T>();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        while (await result.ReadAsync())
                        {
                            if (typeof(T).IsValueType || typeof(T) == typeof(string))
                            {
                                if (result.IsDBNull(0))
                                {
                                    var obj = default(T);
                                    list.Add(obj);
                                }
                                else {
                                    var obj = (T)Convert.ChangeType(result.GetValue(0), typeof(T));
                                    list.Add(obj);
                                }
                            }
                            else 
                            {
                                var obj = Activator.CreateInstance<T>();
                                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                                {
                                    if (!object.Equals(result[prop.Name], DBNull.Value))
                                    {
                                        prop.SetValue(obj, result[prop.Name], null);
                                    }
                                }

                                list.Add(obj);
                            }
                        }
                    }

                    if (outputParameters != null)
                    {
                        // foreach (var outputParam in outputParameters)
                        // {
                        //     outputParameters[outputParam.Key] = command.Parameters[outputParam.Key].Value;
                        // }
                        foreach (var dic in outputParameters)
                        {
                            var value = command.Parameters[dic.Key].Value;

                            if (value != DBNull.Value)
                            {
                                Type? propType = dic.Value?.GetType();
                                if(propType != null)
                                {
                                    var castedValue = Convert.ChangeType(value, propType);
                                    outputParameters[dic.Key] = castedValue;
                                }
                                else
                                {
                                    outputParameters[dic.Key] = value;
                                }
                            }
                            else
                            {
                                outputParameters[dic.Key] = null;
                            }
                        }
                    }
                    
                    connection.Close();
                    return list;
                }
            }
        }

        public void Attach<TEntity>(TEntity entity)
        {
            //this._context.ChangeTracker.TrackGraph(entity, node => {});
            if(entity != null)
            {
                this._context.Attach(entity);
            }
        } 
       
        public void AttachRange(IEnumerable<object> entities)
        {
            if(entities != null)
            {
                this._context.AttachRange(entities);
            }
        } 

        public void Detach<TEntity>(TEntity entity)
        {
            this._context.Entry(entity).State = EntityState.Detached;
        } 

        public void ClearTracking()
        {
            this._context.ChangeTracker.Clear();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this._context.SaveChangesAsync();
        }


        public void Dispose()
        {
            this._context.Dispose();
        }

        
    }
}
