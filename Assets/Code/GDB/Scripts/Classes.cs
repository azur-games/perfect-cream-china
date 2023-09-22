using System.Collections;
using System.Collections.Generic;


namespace BoGD
{

	/// <summary>
	/// Покупаемый элемент
	/// </summary>
	public class InAppItem : IInAppItem
	{
		public string ID
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string Price
		{
			get;
			set;
		}

		public bool Available
		{
			get;
			set;
		}

		public string ISO
		{
			get;
			set;
		}

		public decimal LocalizedPrice
		{
			get;
			set;
		}

		public string TransactionID
		{
			get;
			set;
		}

		public string Receipt
		{
			get;
			set;
		}

		public InAppItem()
		{ 
		}

		public InAppItem(string id, string title, string price, string iso, decimal localizedPrice, string transactionId, bool available, string type)
		{
			ID = id;
			Title = title;
			Price = price;
			Available = available;
			ISO = iso;
			LocalizedPrice = localizedPrice;
			TransactionID = transactionId;
			Type = type;
		}
	}
}

