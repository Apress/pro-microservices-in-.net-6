using System.Collections.Generic;

namespace MessageContracts
{
	public interface IInvoiceCreated
	{
		int InvoiceNumber { get; }
		IInvoiceToCreate InvoiceData { get; }
	}

	public interface IInvoiceToCreate
	{
		int CustomerNumber { get; set; }
		List<InvoiceItems> InvoiceItems { get; set; }
	}

	public class InvoiceItems
	{
		public string Description { get; set; }
		public double Price { get; set; }
		public double ActualMileage { get; set; }
		public double BaseRate { get; set; }
		public bool IsOversized { get; set; }
		public bool IsRefrigerated { get; set; }
		public bool IsHazardousMaterial { get; set; }
	}
}