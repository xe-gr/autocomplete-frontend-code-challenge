using System.Collections.Generic;
using Autocomplete.DataAccess.Entities;
using Autocomplete.DataAccess.Repositories.Interfaces;
using Autocomplete.Service.Controllers;
using Autocomplete.Service.Dto;
using Autocomplete.Service.Errors;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Autocomplete.Service.Tests
{
	public class SearchControllerTests
	{
		[Theory]
		[InlineData(null, "en", Messages.NoKeywordsSpecified)]
		[InlineData("", "en", Messages.NoKeywordsSpecified)]
		[InlineData("a", "en", Messages.ShortKeywordSpecified)]
		[InlineData("key", null, Messages.NoLanguageSpecified)]
		[InlineData("key", "", Messages.NoLanguageSpecified)]
		[InlineData("key", "lang", Messages.InvalidLanguageSpecified)]
		public void NoKeywords(string keywords, string language, string expectedError)
		{
			var controller = new SearchController(null);

			var result = controller.Get(keywords, language, null);

			Assert.NotNull(result);

			var r = result as BadRequestObjectResult;

			Assert.NotNull(r);
			Assert.Equal(expectedError, r.Value);
		}

		[Fact]
		public void NothingFound()
		{
			var repo = CreateMockedRepo(0, 10);
			
			var controller = new SearchController(repo.Object);

			var result = controller.Get("key", "en", null);

			Assert.NotNull(result);

			var r = result as OkObjectResult;

			Assert.NotNull(r);
			
			var dto = r.Value as EntriesDto;

			Assert.NotNull(dto);
			Assert.Empty(dto.Entries);

			repo.Verify(x => x.FindEntries("key", "en", 10), Times.Once);
		}

		[Theory]
		[InlineData(-1, 10)]
		[InlineData(null, 10)]
		[InlineData(1, 1)]
		[InlineData(11, 11)]
		[InlineData(500, 100)]
		public void SpecificNumberOfResultsFound(int? numberRequested, int numberExpected)
		{
			var repo = CreateMockedRepo(numberExpected, numberExpected);

			var controller = new SearchController(repo.Object);

			var result = controller.Get("key", "en", numberRequested);

			Assert.NotNull(result);

			var r = result as OkObjectResult;

			Assert.NotNull(r);

			var dto = r.Value as EntriesDto;

			Assert.NotNull(dto);
			Assert.Equal(numberExpected, dto.Entries.Count);

			repo.Verify(x => x.FindEntries("key", "en", numberExpected), Times.Once);
		}

		private Mock<IEntryRepository> CreateMockedRepo(int numberOfResults, int limitNumber)
		{
			var repo = new Mock<IEntryRepository>(MockBehavior.Strict);

			var results = new List<Entry>();
			for (var i = 1; i <= numberOfResults; i++)
			{
				results.Add(new Entry {Country = "country", Name = "name", Language = "language", Id = i, Keywords = "keywords"});
			}

			repo.Setup(x => x.FindEntries("key", "en", limitNumber)).Returns(results).Verifiable();

			return repo;
		}
	}
}
