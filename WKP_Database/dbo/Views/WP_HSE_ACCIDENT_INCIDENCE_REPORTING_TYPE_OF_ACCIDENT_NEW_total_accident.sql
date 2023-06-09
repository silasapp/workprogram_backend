﻿CREATE VIEW [dbo].[WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_accident]
AS
SELECT     TOP (100) PERCENT SUM(CAST(Frequency AS int)) AS Sum_accident, Year_of_WP
FROM         dbo.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW
GROUP BY Year_of_WP
ORDER BY Year_of_WP