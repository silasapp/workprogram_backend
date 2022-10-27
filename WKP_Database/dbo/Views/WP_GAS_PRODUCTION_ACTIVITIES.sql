<<<<<<< HEAD
﻿/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[WP_GAS_PRODUCTION_ACTIVITIES]
AS
SELECT     Year_of_WP, SUM(CAST(Current_Actual_Year AS int)) AS Actual_Total_Gas_Produced, SUM(CAST(Utilized AS int)) AS Utilized_Gas_Produced, SUM(CAST(Flared AS int))
=======
﻿
/****** Script for SelectTopNRows command from SSMS  ******/
CREATE   VIEW [dbo].[WP_GAS_PRODUCTION_ACTIVITIES]
AS
SELECT     Year_of_WP, SUM(CAST(CAST(Current_Actual_Year AS decimal)AS int)) AS Actual_Total_Gas_Produced, 
SUM(CAST(CAST(Utilized AS decimal) AS int)) AS Utilized_Gas_Produced, SUM(CAST(CAST((Flared) AS decimal)AS  int))
>>>>>>> origin/main
                       AS Flared_Gas_Produced
FROM         dbo.GAS_PRODUCTION_ACTIVITIES
GROUP BY Year_of_WP
