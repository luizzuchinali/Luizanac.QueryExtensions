namespace Luizanac.QueryExtensions.App.Entities
{
	public class Address
	{
		public string City { get; set; }
		public string Street { get; set; }
		public string Number { get; set; }
		public string State { get; set; }

		public Address(string city, string street, string number, string state)
		{
			City = city;
			Street = street;
			Number = number;
			State = state;
		}
	}
}