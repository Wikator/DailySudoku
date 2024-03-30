using Neo4j.Driver;
using Sudoku.App.Enums;
using Sudoku.App.Extensions;
using Sudoku.App.Helpers;
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