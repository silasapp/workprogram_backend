﻿
CREATE VIEW [dbo].[WP_TOTAL_NO_OF_INCIDENCE_BY_YEAR_COMPANYNAME]
AS
SELECT     CompanyName, Year_of_WP, SUM(CAST(Frequency AS decimal)) AS Frequency
FROM         dbo.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW
GROUP BY CompanyName, Year_of_WP