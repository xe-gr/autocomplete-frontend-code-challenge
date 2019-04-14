namespace Autocomplete.DataAccess.Entities
{
	public class Entry
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Keywords { get; set; }
		public virtual string Language { get; set; }
		public virtual string Country { get; set; }
	}
}