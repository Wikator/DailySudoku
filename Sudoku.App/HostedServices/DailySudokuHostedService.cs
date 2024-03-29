
using Sudoku.App.Repositories.Contracts;
using Sudoku.App.Services.Contracts;

namespace Sudoku.App.HostedServices;

public class DailySudokuHostedService(IServiceProvider serviceProvider) : BackgroundService
{
    public IServiceProvider Services { get; } = serviceProvider;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await GenerateDailySudokuAsync(stoppingToken);
    }
    
    private async Task GenerateDailySudokuAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = Services.CreateAsyncScope();
            var scopeServices = scope.ServiceProvider;
        
            var sudokuService = scopeServices.GetRequiredService<ISudokuService>();
            var sudokuRepository = scopeServices.GetRequiredService<ISudokuRepository>();
            
            var board = sudokuService.GenerateBoard();
            var date = DateTime.UtcNow.Date;
            await sudokuRepository.CreateDailySudokuAsync(board, date);

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}