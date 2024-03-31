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

    public async Task<SudokuWithIdAndValidation> GetDailySudokuAsync(string? userId = null, int daysAgo = 0)
    {
        if (userId is null)
        {
            // language=Cypher
            const string query = """
                                 MATCH (s:DailySudoku)
                                 RETURN
                                   s.board AS Board,
                                   s.id AS Id
                                 ORDER BY s.date DESC
                                 SKIP $daysAgo
                                 LIMIT 1
                                 """;

            var parameters = new { daysAgo };
        
            var result = await DataAccess.ExecuteWriteSingleAsync(query, parameters);
            var boardString = result["Board"].As<string>();

            var board = new SudokuBoard<SudokuCell>((row, col) =>
                new SudokuCell
                {
                    Value = (SudokuDigit)int.Parse(boardString[row * 9 + col].ToString()),
                    IsFixed = boardString[row * 9 + col] != '0'
                });

            return new SudokuWithIdAndValidation(Guid.Parse(result["Id"].As<string>()), board);
        }
        else
        {
            // language=Cypher
            const string query = """
                                 MATCH (s:DailySudoku)
                                 MATCH (u:User { id: $userId })
                                 OPTIONAL MATCH (u)-[r:PROGRESS]->(s)
                                 RETURN CASE
                                   WHEN r IS NOT NULL THEN { originalBoard: s.board, userBoard: r.board, id: s.id }
                                   ELSE { board: s.board, id: s.id }
                                 END AS Result
                                 ORDER BY s.date DESC
                                 SKIP $daysAgo
                                 LIMIT 1
                                 """;

            var parameters = new
            {
                daysAgo,
                userId
            };
        
            var result = (await DataAccess.ExecuteWriteSingleAsync(query, parameters))["Result"].As<Dictionary<string, object>>();

            if (result.TryGetValue("board", out var value))
            {
                var boardString = value.As<string>();
                var board = new SudokuBoard<SudokuCell>((row, col) =>
                    new SudokuCell
                    {
                        Value = (SudokuDigit)int.Parse(boardString[row * 9 + col].ToString()),
                        IsFixed = boardString[row * 9 + col] != '0'
                    });

                return new SudokuWithIdAndValidation(Guid.Parse(result["id"].As<string>()), board);
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

                return new SudokuWithIdAndValidation(Guid.Parse(result["id"].As<string>()), board);
            }
        }
    }

    public async Task SaveDailySudokuProgress(string userId, string sudokuId,
        SudokuBoard<SudokuDigit> board, bool isSolved)
    {
        // language=Cypher
        const string query = """
                             MATCH (s:DailySudoku { id: $sudokuId })
                             MATCH (u:User { id: $userId })
                             MERGE (u)-[r:PROGRESS]->(s)
                             SET r.board = $board, r.isSolved = $isSolved
                             """;

        var parameters = new
        {
            sudokuId,
            userId,
            isSolved,
            board = board.Board.ToBoardString()
        };

        await DataAccess.ExecuteWriteAsync(query, parameters);
    }
}