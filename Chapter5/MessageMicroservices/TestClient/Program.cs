using GreenPipes;
using MassTransit;
using MessageContracts;

Console.WriteLine("Waiting while consumers initialize.");
await Task.Delay(3000); //because the consumers need to start first

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost");
    cfg.ReceiveEndpoint("invoice-service-created", e =>
    {
        e.UseInMemoryOutbox();
        e.Consumer<InvoiceCreatedConsumer>(c =>
          c.UseMessageRetry(m => m.Interval(5, new TimeSpan(0, 0, 10))));
    });
});

var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

await busControl.StartAsync(source.Token);
var keyCount = 0;
try
{
    Console.WriteLine("Enter any key to send an invoice request or Q to quit.");

    while (Console.ReadKey(true).Key != ConsoleKey.Q)
    {
        keyCount++;
        await SendRequestForInvoiceCreation(busControl);
        Console.WriteLine($"Enter any key to send an invoice request or Q to quit. {keyCount}");
    }
}
finally
{
    await busControl.StopAsync();
}


static async Task SendRequestForInvoiceCreation(IPublishEndpoint publishEndpoint)
{
    var rnd = new Random();
    await publishEndpoint.Publish<IInvoiceToCreate>(new
    {
        CustomerNumber = rnd.Next(1000, 9999),
        InvoiceItems = new List<InvoiceItems>()
          {
            new InvoiceItems{Description="Tables", Price=Math.Round(rnd.NextDouble()*100,2), ActualMileage = 40, BaseRate = 12.50, IsHazardousMaterial = false, IsOversized = true, IsRefrigerated = false},
            new InvoiceItems{Description="Chairs", Price=Math.Round(rnd.NextDouble()*100,2), ActualMileage = 40, BaseRate = 12.50, IsHazardousMaterial = false, IsOversized = false, IsRefrigerated = false}
          }
    });
}

public class InvoiceCreatedConsumer : IConsumer<IInvoiceCreated>
{
    public async Task Consume(ConsumeContext<IInvoiceCreated> context)
    {
        await Task.Run(() => Console.WriteLine($"Invoice with number: {context.Message.InvoiceNumber} was created."));
    }
}
