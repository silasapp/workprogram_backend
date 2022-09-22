﻿





CREATE VIEW [dbo].[WP_OML_Aggregated_Score_ALL_COMPANIES]
AS

SELECT     CompanyName, Consession_Type, Year_of_WP, INDEX_TYPE , Recalibrated_Scaled_Index_SUM, Weight_SUM, Weighted_Score_SUM, 
                    -- Weighted_Score_SUM / CONVERT(decimal(18, 3), Weight_SUM) AS OML_Aggregated_Score
					   ISNULL( Weighted_Score_SUM / NULLIF(Weight_SUM, 0), 0)  * 100 AS OML_Aggregated_Score
FROM         (SELECT     CompanyName, Consession_Type, Year_of_WP, INDEX_TYPE , SUM(CAST(Recalibrated_Scaled_Index AS NUMERIC(10,2) )) AS Recalibrated_Scaled_Index_SUM, 
                                              SUM(CAST(Weight AS int)) AS Weight_SUM, SUM(CAST(Weighted_Score AS NUMERIC(10,2) )) AS Weighted_Score_SUM
                       FROM          (SELECT     CompanyName, Consession_Type, Year_of_WP, INDEX_TYPE, Recalibrated_Scaled_Index, Weight, Weighted_Score
                                               FROM          dbo.WP_OML_ACQUSITION_INDEX_WEIGHTED_SCORE
                                               --UNION ALL
                                               --SELECT     CompanyName, Consession_Type, Year_of_WP, INDEX_TYPE, Recalibrated_Scaled_Index, Weight, Weighted_Score
                                               --FROM         dbo.WP_OML_COMPLIANCE_INDEX_WEIGHTED_SCORE
                                               UNION ALL
                                               SELECT     CompanyName, Consession_Type, Year_of_WP, INDEX_TYPE, Recalibrated_Scaled_Index, Weight, Weighted_Score
                                               FROM         dbo.WP_OML_EXPLORATORY_DRILLING_INDEX_WEIGHTED_SCORE
                                               UNION ALL
                                               SELECT     CompanyName, Consession_Type, Year_of_WP, INDEX_TYPE, Recalibrated_Scaled_Index, Weight, Weighted_Score
                                               FROM         dbo.WP_OML_DISCOVERY_INDEX_WEIGHTED_SCORE
                                               UNION ALL
                                               SELECT     CompanyName, Consession_Type, Year AS YEAR_OF_WP, INDEX_TYPE, Recalibrated_Scaled_Index, Weight, Weighted_Score
                                               FROM         dbo.WP_OML_CONCESSION_RENTALS_INDEX_WEIGHTED_SCORE) AS SUM_ALL
                       GROUP BY CompanyName, Consession_Type, Year_of_WP , INDEX_TYPE) AS OML_Aggregated_Score
