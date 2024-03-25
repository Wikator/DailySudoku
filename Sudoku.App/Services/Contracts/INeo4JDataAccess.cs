using Neo4j.Driver;

namespace Sudoku.App.Services.Contracts;

public interface INeo4JDataAccess
{
    Task<IRecord> ExecuteReadSingleAsync(string query, object parameters);
    Task<IRecord> ExecuteWriteSingleAsync(string query, object parameters);
    Task<List<IRecord>> ExecuteReadListAsync(string query, object parameters);
    Task<List<IRecord>> ExecuteWriteListAsync(string query, object parameters);
    Task ExecuteWriteAsync(string query, object parameters);
}
