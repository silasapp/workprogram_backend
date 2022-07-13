/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[WP_GAS_PRODUCTION_ACTIVITIES_penalty_payment]
AS
SELECT     CompanyName, Year_of_WP, SUM(CAST(Flared AS int)) AS Flared, SUM(CAST(penaltyfeepaid AS int)) AS penaltyfeepaid, SUM(CAST(Amount_Paid AS int)) 
                      AS Amount_Paid
FROM         dbo.GAS_PRODUCTION_ACTIVITIES
GROUP BY CompanyName, Year_of_WP
