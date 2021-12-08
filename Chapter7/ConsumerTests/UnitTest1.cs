using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit;
using Xunit;
using MassTransit.Testing;
using MessageContracts;
using PaymentMicroservice;

namespace ConsumerTests
{
	public class UnitTest1
	{
		[Fact]
		public async Task Verify_InvoiceCreatedMessage_Consumed()
		{
			var harness = new InMemoryTestHarness();
			var consumerHarness = harness.Consumer<InvoiceCreatedConsumer>();
			await harness.Start();

			try
			{
				await harness.Bus.Publish<IInvoiceCreated>(
				new {InvoiceNumber = InVar.Id });

				//verify endpoint consumed the message
				Assert.True(await harness
					.Consumed.Any<IInvoiceCreated>());

				//verify the real consumer consumed the message
				Assert.True(await consumerHarness
					.Consumed.Any<IInvoiceCreated>());

				//verify there was only one message published
				harness.Published.Select<IInvoiceCreated>()
					.Count().Should().Be(1);
			}
			finally
			{
				await harness.Stop();
			}
		}
	}
}
