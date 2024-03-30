
using Sudoku.App.Repositories.Contracts;
using Sudoku.App.Services.Contracts;

namespace Sudoku.App.HostedServices;

public class DailySudokuHostedService(IServiceProvider serviceProvider) : BackgroundService
{
    private IServiceProvider Services { get; } = serviceProvider;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await GenerateDailySudokuAsync(stoppingToken);
    }
    
    private async Task GenerateDailySudokuAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextRunTime = now.Date.Add(TimeSpan.FromHours(11));
            
            if (nextRunTime < now)
                nextRunTime = nextRunTime.AddDays(1);
            
            var delay = nextRunTime - now;
            
            await Task.Delay(delay, stoppingToken);

            await using var scope = Services.CreateAsyncScope();
            var scopeServices = scope.ServiceProvider;
    
            var sudokuService = scopeServices.GetRequiredService<ISudokuService>();
            var sudokuRepository = scopeServices.GetRequiredService<ISudokuRepository>();
        
            var board = sudokuService.GenerateBoard();
            var date = DateTime.UtcNow.Date;
            await sudokuRepository.CreateDailySudokuAsync(board, date);
        }
    }
}