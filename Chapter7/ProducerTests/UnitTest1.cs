using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MassTransit.Testing;
using MessageContracts;
using Xunit;
using InvoiceMicroservice;

namespace ProducerTests
{
	public class UnitTest1
	{
		[Fact]
		public async Task Verify_InvoiceToCreateCommand_Consumed()
		{
			//Verify that we are receiving and reacting
			//to a command to create an invoice

			var harness = new InMemoryTestHarness();
			var consumerHarness = harness.Consumer<EventConsumer>();

			await harness.Start();

			try
			{
				await harness.InputQueueSendEndpoint
					.Send<IInvoiceToCreate>(
					new {
						CustomerNumber = 19282,
						InvoiceItems = new List<InvoiceItems>()
						{
							new InvoiceItems
							{
								Description="Tables",
								Price=Math.Round(1020.99),
								ActualMileage = 40,
								BaseRate = 12.50,
								IsHazardousMaterial = false,
								IsOversized = true,
								IsRefrigerated = false
							}
						}
					});

				//verify endpoint consumed the message
				Assert.True(await harness
					.Consumed.Any<IInvoiceToCreate>());

				//verify the real consumer consumed the message
				Assert.True(await consumerHarness
					.Consumed.Any<IInvoiceToCreate>());

				//verify that a new message was published
				//because of the new invoice being created
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
