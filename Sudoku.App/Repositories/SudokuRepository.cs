using Neo4j.Driver;
using Sudoku.App.Enums;
using Sudoku.App.Extensions;
using Sudoku.App.Helpers;
using Sudoku.App.Models;
using Sudoku.App.Repositories.Contracts;
using Sudoku.App.Services.Contracts;

namespace Sudoku.App.Repositories;

public class SudokuRepository(INeo4JDataAccess dataAccess) : ISudokuRepository
{
    private INeo4JDataAccess DataAccess { get; } = dataAccess;
    
    public async Task CreateSudokuAsync(SudokuDigit[,] board, Solutions solutions, string userId)
    {
        // language=Cypher
        const string query = """
                             MATCH (u:User {id: $userId})
                             MERGE (s:Sudoku {
                                id: $id,
                                solutions: $solutions,
                                board: $board
                             })<-[:SAVED]-(u)
                             """;
        
        var parameters = new
        {
            id = Guid.NewGuid().ToString(),
            board = board.ToBoardString(),
            solutions = (int)solutions,
            userId
        };
        
        await DataAccess.ExecuteWriteAsync(query, parameters);
    }

    public async Task<PagedResult<SudokuWithId>> GetUserPagedSudoku(string userId, int pageNumber,
        int pageSize)
    {
        // language=Cypher
        const string query = """
                             MATCH (:User { id: $userId })-[:SAVED]->(s:Sudoku)
                             WITH COLLECT({ id: s.id, board: s.board }) AS sudoku
                             RETURN SIZE(sudoku) AS TotalCount, sudoku[$start..$end] AS Items
                             """;

        var parameters = new
        {
            userId,
            // skip = pageSize * (pageNumber - 1),
            // limit = pageSize
            start = pageSize * (pageNumber - 1),
            end = pageSize * pageNumber
        };

        var record = await DataAccess.ExecuteReadSingleAsync(query, parameters);
        var totalCount = record["TotalCount"].As<int>();

        var items = record["Items"].As<List<Dictionary<string, object>>>().Select(item =>
        {
            var boardString = item["board"].As<string>();
            var board = new SudokuBoard<SudokuDigit>((row, col) =>
                    (SudokuDigit)int.Parse(boardString[row * 9 + col].ToString())
                );
            return new SudokuWithId(Guid.Parse(item["id"].As<string>()), board);
        }).ToList();

        return new PagedResult<SudokuWithId>(items, pageNumber, pageSize, totalCount);
    }

    public async Task<SudokuBoard<SudokuCell>?> GetSudoku(string id)
    {
        try
        {
            // language=Cypher
            const string query = """
                                 MATCH (s:Sudoku { id: $id })
                                 RETURN s.board AS Board
                                 """;

            var parameters = new { id };

            var record = await DataAccess.ExecuteReadSingleAsync(query, parameters);
            var boardString = record["Board"].As<string>();

            return new SudokuBoard<SudokuCell>((row, col) =>
                new SudokuCell
                {
                    Value = (SudokuDigit)int.Parse(boardString[row * 9 + col].ToString()),
                    IsFixed = boardString[row * 9 + col] != '0'
                });
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public async Task CreateDailySudokuAsync(SudokuBoard<SudokuDigit> board, DateTime date)
    {
        // language=Cypher
        const string query = """
                             CREATE (s:DailySudoku {
                                id: $id,
                                date: $date,
                                board: $board
                             })<-[:CREATED]-(u)
                             """;

        var parameters = new
        {
            id = Guid.NewGuid().ToString(),
            board = board.Board.ToBoardString(),
            date
        };
        
        await DataAccess.ExecuteWriteAsync(query, parameters);
    }

    public async Task<SudokuBoard<SudokuCell>> GetLatestDailySudokuAsync()
    {
        // language=Cypher
        const string query = """
                             MATCH (s:DailySudoku)
                             RETURN
                               s.board AS Board
                             ORDER BY s.date DESC
                             LIMIT 1
                             """;
        
        var result = await DataAccess.ExecuteWriteSingleAsync(query, new object());
        var boardString = result["Board"].As<string>();

        return new SudokuBoard<SudokuCell>((row, col) =>
            new SudokuCell
            {
                Value = (SudokuDigit)int.Parse(boardString[row * 9 + col].ToString()),
                IsFixed = boardString[row * 9 + col] != '0'
            });
    }
}