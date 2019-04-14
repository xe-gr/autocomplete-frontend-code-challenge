using System;
using Autocomplete.DataAccess.Repositories;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Caches.RtMemoryCache;
using NHibernate.Cfg;

namespace Autocomplete.DataAccess
{
	public sealed class SessionManager
	{
		private const string SessionKey = "Autocomplete.API.DataAccess.UoW.Session";
		private const string TransactionKey = "Autocomplete.API.DataAccess.UoW.Transaction";
		private static readonly Lazy<SessionManager> LazyInstance = new Lazy<SessionManager>(() => new SessionManager());

		private readonly ISessionFactory _sessionFactory;

		public static SessionManager DefaultManager => LazyInstance.Value;

		private static ISession ThreadSession
		{
			get => (ISession)CallContext.GetData(SessionKey);
			set => CallContext.SetData(SessionKey, value);
		}

		private SessionManager()
		{
			_sessionFactory = BuildSessionFactory();
		}

		public void CloseSession()
		{
			var session = ThreadSession;
			ThreadSession = null;

			if (session != null && session.IsOpen)
			{
				session.Close();
			}
		}

		public ISession GetSession()
		{
			var session = ThreadSession;

			if (session == null)
			{
				session = _sessionFactory.OpenSession();
				ThreadSession = session;
			}

			return ThreadSession;
		}

		public ISession GetSession(bool tracked)
		{
			if (tracked)
			{
				return GetSession();
			}

			return _sessionFactory.OpenSession();
		}

		public void SetSession(ISession session, Repository repo)
		{
			repo.Session = session;
		}

		private static ISessionFactory BuildSessionFactory()
		{
			return Fluently.Configure()
							.Database(
								MsSqlConfiguration.MsSql2012.ConnectionString(Configuration.ConnectionString).AdoNetBatchSize(256))
							.Mappings(m => m.FluentMappings.AddFromAssemblyOf<SessionManager>())
							.Cache(x =>
							{
								x.UseQueryCache().ProviderClass<RtMemoryCacheProvider>();
							})
							.ExposeConfiguration(x => x.SessionFactory().Caching.Through<RtMemoryCacheProvider>().WithDefaultExpiration(60))
							.BuildSessionFactory();
		}
	}
}