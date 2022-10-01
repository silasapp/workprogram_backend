<<<<<<< HEAD
﻿CREATE VIEW [dbo].[WP_WORKOVERS_RECOMPLETION_JOBS]
AS
SELECT     SUM(CAST(Current_year_Actual_Number_data AS int)) AS Current_year_Actual_Number_data, SUM(CAST(Proposed_year_data AS int)) AS Proposed_year_data, 
=======
﻿
CREATE VIEW [dbo].[WP_WORKOVERS_RECOMPLETION_JOBS]
AS
SELECT     SUM(CAST(CAST(Current_year_Actual_Number_data AS decimal )AS int)) AS Current_year_Actual_Number_data,
SUM(CAST(CAST(Proposed_year_data AS decimal)AS int)) AS Proposed_year_data, 
>>>>>>> origin/main
                      Year_of_WP
FROM         dbo.WORKOVERS_RECOMPLETION_JOBS
GROUP BY Year_of_WP
