using System.Collections.Generic;
using Autocomplete.DataAccess.Entities;
using Autocomplete.DataAccess.Repositories.Interfaces;
using NHibernate;

namespace Autocomplete.DataAccess.Repositories
{
	public class EntryRepository : Repository, IEntryRepository
	{
		public EntryRepository()
		{
		}

		public EntryRepository(ISession session) : base(session)
		{
		}

		public IList<Entry> FindEntries(string keyword, string language, int limit)
		{
			return Session
				.CreateSQLQuery($"exec SearchLocations @keyword=N'{keyword}',@lang=N'{language}',@num={limit}")
				.AddEntity(typeof(Entry))
				.SetCacheable(true)
				.List<Entry>();
		}
	}
}