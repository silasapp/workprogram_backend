<<<<<<< HEAD
﻿CREATE VIEW [dbo].[WP_INITIAL_WELL_COMPLETION_JOBS]
AS
SELECT     SUM(CAST(Current_year_Actual_Number AS int)) AS Current_year_Actual_Number, SUM(CAST(Proposed_year_data AS int)) AS Proposed_year_data, 
=======
﻿
CREATE VIEW [dbo].[WP_INITIAL_WELL_COMPLETION_JOBS]
AS
SELECT     SUM(CAST(CAST(Current_year_Actual_Number AS decimal)AS int)) AS Current_year_Actual_Number,
		   SUM(CAST(CAST(Proposed_year_data AS decimal) AS int)) AS Proposed_year_data, 
>>>>>>> origin/main
                      Year_of_WP
FROM         dbo.INITIAL_WELL_COMPLETION_JOBS
GROUP BY Year_of_WP
