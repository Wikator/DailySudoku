using Neo4j.Driver;
using Sudoku.App.Services.Contracts;

namespace Sudoku.App.Services;

public class Neo4JDataAccess(IDriver driver) : INeo4JDataAccess, IAsyncDisposable
{
    private IAsyncSession Session { get; } = driver.AsyncSession();
    
    public async Task<IRecord> ExecuteReadSingleAsync(string query, object parameters)
    {
        return await Session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, parameters);
            return await cursor.SingleAsync();
        });
    }

    public async Task<IRecord> ExecuteWriteSingleAsync(string query, object parameters)
    {
        return await Session.ExecuteWriteAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, parameters);
            return await cursor.SingleAsync();
        });
    }

    public async Task<List<IRecord>> ExecuteReadListAsync(string query, object parameters)
    {
        return await Session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, parameters);
            return await cursor.ToListAsync();
        });
    }

    public async Task<List<IRecord>> ExecuteWriteListAsync(string query, object parameters)
    {
        return await Session.ExecuteWriteAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, parameters);
            return await cursor.ToListAsync();
        });
    }

    public async Task ExecuteWriteAsync(string query, object parameters)
    {
        await Session.ExecuteWriteAsync(async tx =>
        {
            await tx.RunAsync(query, parameters);
        });
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await Session.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    void IDisposable.Dispose()
    {
        Session.Dispose();
        GC.SuppressFinalize(this);
    }
}