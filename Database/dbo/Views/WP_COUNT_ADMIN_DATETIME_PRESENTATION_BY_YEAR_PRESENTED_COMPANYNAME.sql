﻿CREATE VIEW [dbo].[WP_COUNT_ADMIN_DATETIME_PRESENTATION_BY_YEAR_PRESENTED_COMPANYNAME]
AS
SELECT COUNT(PRESENTED) AS count, Year, CompanyName ,PRESENTED
FROM         dbo.ADMIN_DATETIME_PRESENTATION
GROUP BY Year, CompanyName , PRESENTED