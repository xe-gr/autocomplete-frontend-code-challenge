using NHibernate;

namespace Autocomplete.DataAccess.Repositories
{
	public class Repository
	{
		public ISession Session { get; set; }

		public Repository()
		{
			Session = SessionManager.DefaultManager.GetSession();
		}

		public Repository(ISession session)
		{
			Session = session;
		}
	}
}