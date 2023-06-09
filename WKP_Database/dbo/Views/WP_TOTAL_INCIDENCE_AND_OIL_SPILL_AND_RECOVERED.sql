﻿
CREATE VIEW [dbo].[WP_TOTAL_INCIDENCE_AND_OIL_SPILL_AND_RECOVERED]
AS
SELECT     dbo.WP_TOTAL_NO_OF_INCIDENCE_BY_YEAR_COMPANYNAME.CompanyName, dbo.WP_TOTAL_NO_OF_INCIDENCE_BY_YEAR_COMPANYNAME.Year_of_WP, 
                      dbo.WP_TOTAL_NO_OF_INCIDENCE_BY_YEAR_COMPANYNAME.Frequency, 
                      dbo.WP_TOTAL_NO_OF_OIL_SPILLED_AND_RECOVERED_BY_YEAR_COMPANYNAME.Total_Quantity_Spilled, 
                      dbo.WP_TOTAL_NO_OF_OIL_SPILLED_AND_RECOVERED_BY_YEAR_COMPANYNAME.Total_Quantity_Recovered
FROM         dbo.WP_TOTAL_NO_OF_INCIDENCE_BY_YEAR_COMPANYNAME INNER JOIN
                      dbo.WP_TOTAL_NO_OF_OIL_SPILLED_AND_RECOVERED_BY_YEAR_COMPANYNAME ON 
                      dbo.WP_TOTAL_NO_OF_INCIDENCE_BY_YEAR_COMPANYNAME.CompanyName = dbo.WP_TOTAL_NO_OF_OIL_SPILLED_AND_RECOVERED_BY_YEAR_COMPANYNAME.CompanyName
                       AND 
                      dbo.WP_TOTAL_NO_OF_INCIDENCE_BY_YEAR_COMPANYNAME.Year_of_WP = dbo.WP_TOTAL_NO_OF_OIL_SPILLED_AND_RECOVERED_BY_YEAR_COMPANYNAME.Year_of_WP