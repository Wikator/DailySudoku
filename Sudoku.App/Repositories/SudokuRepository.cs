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

    public async Task RemoveSudokuFromSaved(string userId, string sudokuId)
    {
        // language=Cypher
        const string query = """
                             MATCH (:User { id: $userId })-[r:SAVED]->(:Sudoku { id: $sudokuId })
                             DELETE r
                             """;

        var parameters = new { userId, sudokuId };
        await DataAccess.ExecuteWriteAsync(query, parameters);
    }

    public async Task<PagedResult<SudokuWithId>> GetUserPagedSudoku(string userId, int pageNumber,
        int pageSize)
    {
        // language=Cypher
        const string query = """
                             MATCH (:User { id: $userId })-[:SAVED]->(s:Sudoku)
                             WITH COLLECT({ id: s.id, board: s.board, solutions: s.solutions }) AS sudoku
                             RETURN SIZE(sudoku) AS TotalCount, sudoku[$start..$end] AS Items
                             """;

        var parameters = new
        {
            userId,
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
            return new SudokuWithId(Guid.Parse(item["id"].As<string>()), board, (Solutions)item["solutions"].As<int>());
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

    public async Task CreateDailySudokuAsync(SudokuBoard<SudokuDigit> board, DateOnly date)
    {
        // language=Cypher
        const string query = """
                             CREATE (s:DailySudoku {
                                id: $id,
                                date: $date,
                                board: $board
                             })
                             """;

        var parameters = new
        {
            id = Guid.NewGuid().ToString(),
            board = board.Board.ToBoardString(),
            date
        };
        
        await DataAccess.ExecuteWriteAsync(query, parameters);
    }

    public async Task<DailySudokuWithProgress> GetDailySudokuAsync(string? userId, DateOnly date)
    {
        if (userId is null)
        {
            // language=Cypher
            const string query = """
                                 MATCH (s:DailySudoku { date: $date })
                                 RETURN
                                   s.board AS Board,
                                   s.id AS Id
                                 """;

            var parameters = new { date };
        
            var result = await DataAccess.ExecuteReadSingleAsync(query, parameters);
            var boardString = result["Board"].As<string>();

            var board = new SudokuBoard<SudokuCell>((row, col) =>
                new SudokuCell
                {
                    Value = (SudokuDigit)int.Parse(boardString[row * 9 + col].ToString()),
                    IsFixed = boardString[row * 9 + col] != '0'
                });

            return new DailySudokuWithProgress(Guid.Parse(result["Id"].As<string>()), 
                TimeSpan.Zero, board);
        }
        else
        {
            // language=Cypher
            const string query = """
                                 MATCH (s:DailySudoku { date: $date })
                                 MATCH (u:User { id: $userId })
                                 OPTIONAL MATCH (u)-[r:PROGRESS]->(s)
                                 RETURN CASE
                                   WHEN r IS NOT NULL THEN { originalBoard: s.board, seconds: r.seconds, userBoard: r.board, id: s.id }
                                   ELSE { board: s.board, seconds: 0, id: s.id }
                                 END AS Result
                                 """;

            var parameters = new
            {
                date,
                userId
            };
        
            var result = (await DataAccess.ExecuteReadSingleAsync(query, parameters))["Result"].As<Dictionary<string, object>>();
            
            if (result.TryGetValue("board", out var value))
            {
                var boardString = value.As<string>();
                var board = new SudokuBoard<SudokuCell>((row, col) =>
                    new SudokuCell
                    {
                        Value = (SudokuDigit)int.Parse(boardString[row * 9 + col].ToString()),
                        IsFixed = boardString[row * 9 + col] != '0'
                    });

                return new DailySudokuWithProgress(Guid.Parse(result["id"].As<string>()),
                    TimeSpan.FromSeconds(result["seconds"].As<int>()), board);
            }
            else
            {
                var originalBoardString = result["originalBoard"].As<string>();
                var userBoardString = result["userBoard"].As<string>();
            
                var board = new SudokuBoard<SudokuCell>((row, col) =>
                    new SudokuCell
                    {
                        Value = (SudokuDigit)int.Parse(userBoardString[row * 9 + col].ToString()),
                        IsFixed = originalBoardString[row * 9 + col] != '0'
                    });

                return new DailySudokuWithProgress(Guid.Parse(result["id"].As<string>()),
                    TimeSpan.FromSeconds(result["seconds"].As<int>()), board);
            }
        }
    }

    public async Task<bool> DailySudokuExists(DateOnly date)
    {
        try
        {
            // language=Cypher
            const string query = """
                                 MATCH (s:DailySudoku { date: $date })
                                 RETURN COUNT(s) > 0 AS Exists
                                 """;

            var parameters = new { date };

            var result = await DataAccess.ExecuteReadSingleAsync(query, parameters);
            return result["Exists"].As<bool>();
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public async Task<List<DailySudokuStatus>> GetDailySudokuStatuses(string? userId)
    {
        if (userId is not null)
        {
            // language=Cypher
            const string query = """
                                 MATCH (s:DailySudoku)
                                 MATCH (u:User { id: $userId })
                                 OPTIONAL MATCH (u)-[r:PROGRESS]->(s)
                                 RETURN CASE
                                   WHEN r IS NOT NULL THEN { id: s.id, date: s.date, status: CASE WHEN r.isSolved THEN 0 ELSE 1 END }
                                   ELSE { id: s.id, date: s.date, status: 2 }
                                 END AS Status
                                 ORDER BY s.date DESC
                                 """;

            var parameters = new { userId };
            var records = await DataAccess.ExecuteReadListAsync(query, parameters);
            return records.Select(record =>
            {
                var dictionary = record["Status"].As<Dictionary<string, object>>();
                return new DailySudokuStatus(Guid.Parse(dictionary["id"].As<string>()),
                    DateOnly.FromDateTime(dictionary["date"].As<DateTime>()),
                    (BoardStatus)dictionary["status"].As<int>());
            }).ToList();
        }
        else
        {
            // language=Cypher
            const string query = """
                                 MATCH (s:DailySudoku)
                                 RETURN 
                                   s.id AS Id,
                                   s.date AS Date
                                 ORDER BY s.date DESC
                                 """;

            var records = await DataAccess.ExecuteReadListAsync(query, new object());
            return records.Select(record => new DailySudokuStatus(Guid.Parse(record["Id"].As<string>()),
                    DateOnly.FromDateTime(record["Date"].As<DateTime>()), BoardStatus.Empty)).ToList();
            
        }
    }

    public async Task SaveDailySudokuProgress(string userId, string sudokuId,
        SudokuBoard<SudokuDigit> board, bool isSolved, TimeSpan timeTaken)
    {
        // language=Cypher
        const string query = """
                             MATCH (s:DailySudoku { id: $sudokuId })
                             MATCH (u:User { id: $userId })
                             MERGE (u)-[r:PROGRESS]->(s)
                             SET r.board = $board, r.isSolved = $isSolved, r.seconds = $seconds
                             """;

        var parameters = new
        {
            sudokuId,
            userId,
            isSolved,
            seconds = (int)timeTaken.TotalSeconds,
            board = board.Board.ToBoardString()
        };

        await DataAccess.ExecuteWriteAsync(query, parameters);
    }
}