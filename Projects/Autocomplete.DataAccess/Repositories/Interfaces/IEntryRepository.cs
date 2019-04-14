using System.Collections.Generic;
using Autocomplete.DataAccess.Entities;

namespace Autocomplete.DataAccess.Repositories.Interfaces
{
	public interface IEntryRepository
	{
		IList<Entry> FindEntries(string keyword, string language, int limit);
	}
}