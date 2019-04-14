using System.Linq;
using Autocomplete.DataAccess.Repositories.Interfaces;
using Autocomplete.Service.Dto;
using Autocomplete.Service.Errors;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Autocomplete.Service.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SearchController : ControllerBase
	{
		private readonly IEntryRepository _entryRepository;
		private readonly Logger _log;

		public SearchController(IEntryRepository entryRepository)
		{
			_entryRepository = entryRepository;
			_log = LogManager.GetCurrentClassLogger();
		}

		/// <summary>Returns results matching searching keywords and language.</summary>
		/// <response code="200">Success.</response>
		/// <response code="400">Errors in specified keywords and/or language.</response>
		/// <response code="500">Internal server error.</response>
		/// <param name="keywords">The keywords to search for.</param>
		/// <param name="language">The language to use during the search.</param>
		/// <param name="limit">Optional limit to number of results to return. If not specified, default is 10.</param>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Get([FromQuery] string keywords, string language, int? limit)
		{
			if (string.IsNullOrEmpty(keywords))
			{
				_log.Error("No keyword specified");
				return BadRequest(Messages.NoKeywordsSpecified);
			}

			if (string.IsNullOrEmpty(language))
			{
				_log.Error("No language specified");
				return BadRequest(Messages.NoLanguageSpecified);
			}

			if (keywords.Length < 2)
			{
				_log.Error("Short keyword specified");
				return BadRequest(Messages.ShortKeywordSpecified);
			}

			if (language.Length != 2)
			{
				_log.Error("Invalid language specified");
				return BadRequest(Messages.InvalidLanguageSpecified);
			}

			if (!limit.HasValue)
			{
				limit = 10;
			}
			else if (limit <= 0)
			{
				limit = 10;
			}
			else if (limit > 100)
			{
				limit = 100;
			}

			_log.Debug($"Searching for [{keywords}] with language [{language}]");
			var entries = _entryRepository.FindEntries(keywords, language, limit.Value);
			_log.Debug($"{entries.Count} results found");

			var returned = new EntriesDto
			{
				Entries = entries.Select(entry => new EntryDto
				{
					Language = entry.Language, Country = entry.Country, Name = entry.Name
				}).ToList()
			};

			return Ok(returned);
		}
	}
}
