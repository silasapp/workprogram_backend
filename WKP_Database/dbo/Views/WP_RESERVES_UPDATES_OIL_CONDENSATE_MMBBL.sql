/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[WP_RESERVES_UPDATES_OIL_CONDENSATE_MMBBL]
AS
SELECT     Year_of_WP, SUM(CAST(Reserves_as_at_MMbbl AS int)) AS Reserves_as_at_MMbbl, SUM(CAST(Reserves_as_at_MMbbl_gas AS int)) 
                      AS Reserves_as_at_MMbbl_gas, SUM(CAST(Reserves_as_at_MMbbl_condensate AS int)) AS Reserves_as_at_MMbbl_condensate
FROM         dbo.RESERVES_UPDATES_OIL_CONDENSATE_MMBBL
GROUP BY Year_of_WP
