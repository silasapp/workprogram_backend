﻿CREATE VIEW [dbo].[WP_COUNT_ADMIN_DATETIME_PRESENTATION_BY_YEAR_PRESENTED_CATEGORY]
AS
SELECT COUNT(PRESENTED) AS count, Year ,PRESENTED
FROM         dbo.ADMIN_DATETIME_PRESENTATION
GROUP BY Year , PRESENTED
