USE [AutoComplete]
GO
/****** Object:  FullTextCatalog [locations_catalog]    Script Date: 14/4/2019 2:58:43 ðì ******/
CREATE FULLTEXT CATALOG [locations_catalog] WITH ACCENT_SENSITIVITY = OFF
GO
/****** Object:  Table [dbo].[Locations]    Script Date: 14/4/2019 2:58:43 ðì ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Locations](
	[id] [int] NOT NULL,
	[name] [nvarchar](500) NOT NULL,
	[keywords] [nvarchar](500) NOT NULL,
	[language] [nvarchar](2) NOT NULL,
	[country] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Locations]    Script Date: 14/4/2019 2:58:43 ðì ******/
CREATE NONCLUSTERED INDEX [IX_Locations] ON [dbo].[Locations]
(
	[language] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  FullTextIndex     Script Date: 14/4/2019 2:58:43 ðì ******/
CREATE FULLTEXT INDEX ON [dbo].[Locations](
[keywords] LANGUAGE 'Neutral')
KEY INDEX [PK_Locations]ON ([locations_catalog], FILEGROUP [PRIMARY])
WITH (CHANGE_TRACKING = MANUAL, STOPLIST = SYSTEM)

GO
/****** Object:  StoredProcedure [dbo].[SearchLocations]    Script Date: 14/4/2019 2:58:43 ðì ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SearchLocations]
	@keyword nvarchar(250),
	@lang nvarchar(2),
	@num int
AS
BEGIN
    SET @keyword = '"' + @keyword + '*"'

	SELECT TOP (@num) Locations.id, locations.name, Locations.keywords, locations.language, locations.country
	from locations inner join containstable (Locations, keywords, @keyword, 1000) as key_tbl
	on key_tbl.[KEY] = locations.id
	where language = @lang
END
GO
