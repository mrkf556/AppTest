using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Basket.ExpireBaskets;

namespace StoreApp.Infrastructure.Services.DomainEvents;

public   class BasketExpirationBackgroundService : BackgroundService
{

    //نکته بسیار مهم چون سرویس بک گراند سینگلتون می باشد نمی توان یک اسکوپ را به ان اضافه کرد اما با سرویس زیر این عمل را ممکن می کند
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BasketExpirationBackgroundService> _logger;

    public BasketExpirationBackgroundService(IServiceScopeFactory scopeFactory,ILogger<BasketExpirationBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ///دوره زمانی هر یک دقیقه یک بار درخواست send را ارسال کنیم
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var commandDispatcher =scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

                await commandDispatcher.Send(new ExpireBasketsCommand(),stoppingToken);
            }
            catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"خطا در اجرای ExpireBasketsCommand");
            }
        }
    }
}