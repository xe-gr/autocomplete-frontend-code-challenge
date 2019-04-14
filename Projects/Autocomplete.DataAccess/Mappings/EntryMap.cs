using Autocomplete.DataAccess.Entities;
using FluentNHibernate.Mapping;

namespace Autocomplete.DataAccess.Mappings
{
	public class EntryMap : ClassMap<Entry>
	{
		public EntryMap()
		{
			Table("Locations");
			Id(x => x.Id);
			Map(x => x.Country);
			Map(x => x.Keywords);
			Map(x => x.Language);
			Map(x => x.Name);
			Cache.ReadOnly();
		}
	}
}