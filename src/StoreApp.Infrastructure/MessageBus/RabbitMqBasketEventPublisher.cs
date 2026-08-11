using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using StoreApp.Application.Abstractions.MessageBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace StoreApp.Infrastructure.MessageBus
{
    public   class RabbitMqBasketEventPublisher : IBasketEventPublisher
    {
        private IConnection? _connection;
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _connectionFactory;

        public RabbitMqBasketEventPublisher(IConfiguration configuration)
        {
            _configuration = configuration;

            _connectionFactory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"],
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };
        }
        public async Task PublishAsync<TEvent>(TEvent @event,CancellationToken cancellationToken = default)
        {
            var channel = await GetChannelAsync();

            await DeclareExchangeAsync(channel);

            var message = JsonSerializer.Serialize(@event);

            var body = Encoding.UTF8.GetBytes(message);
            //به دلیل استفاده از fanout  دیگر routing key را وارد نمیکنیم
            await channel.BasicPublishAsync(exchange: "basket.events",routingKey: string.Empty,body: body,cancellationToken: cancellationToken);
        }
        private async Task DeclareExchangeAsync(IChannel channel)
        {
            await channel.ExchangeDeclareAsync(exchange: "basket.events",
                type: ExchangeType.Fanout,durable: true,autoDelete: false);
        }
        private async Task<IChannel> GetChannelAsync()
        {
            var connection = await GetConnectionAsync();

            return await connection.CreateChannelAsync();
        }
        private async Task<IConnection> GetConnectionAsync()
        {
            if (_connection is not null && _connection.IsOpen)
                return _connection;

            _connection = await _connectionFactory.CreateConnectionAsync();

            return _connection;
        }
    }
}
