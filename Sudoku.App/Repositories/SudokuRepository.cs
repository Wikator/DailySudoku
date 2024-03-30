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
                             CREATE (s:Sudoku {
                                id: $id,
                                solutions: $solutions,
                                board: $board
                             })<-[:CREATED]-(u)
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

    public async Task<List<SudokuWithId>> GetUserSudoku(string userId)
    {
        // language=Cypher
        const string query = """
                             MATCH (:User { id: $userId })-[:CREATED]->(s:Sudoku)
                             RETURN s.board AS Board, s.id AS Id
                             """;

        var parameters = new
        {
            userId
        };

        var records = await DataAccess.ExecuteReadListAsync(query, parameters);

        return records.Select(record =>
        {
            var boardString = record["Board"].As<string>();
            var board = new SudokuBoard<SudokuDigit>((row, col) =>
                    (SudokuDigit)int.Parse(boardString[row * 9 + col].ToString())
                );
            return new SudokuWithId(Guid.Parse(record["Id"].As<string>()), board);
        }).ToList();
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