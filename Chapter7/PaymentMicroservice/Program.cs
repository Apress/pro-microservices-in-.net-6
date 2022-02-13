using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MessageContracts;

namespace PaymentMicroservice
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
			{
				cfg.Host("10.0.75.1");
				cfg.ReceiveEndpoint("payment-service", e =>
				{
					e.Consumer<InvoiceCreatedConsumer>(c =>
						c.UseMessageRetry(m => m.Interval(5, new TimeSpan(0, 0, 10))));
				});
			});

			var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
			await busControl.StartAsync(source.Token);
			Console.WriteLine("Payment Microservice Now Listening");

			try
			{
				while (true)
				{
					//sit in while loop listening for messages
					await Task.Delay(100);
				}
			}
			finally
			{
				await busControl.StopAsync();
			}
		}
	}

	public class InvoiceCreatedConsumer : IConsumer<IInvoiceCreated>
	{
		public async Task Consume(ConsumeContext<IInvoiceCreated> context)
		{
			await Task.Run(() =>
				Console.WriteLine($"Received message for invoice number: {context.Message.InvoiceNumber}"));
		}
	}
}