



CREATE VIEW [dbo].[WP_OML_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIES]
AS
/****** Script for SelectTopNRows command from SSMS  ******/
SELECT  [CompanyName]
  ,[INDEX_TYPE]
      ,[Unscaled_Score]
      ,[Unscaled_Score_sum]
      ,[Scaled_by_Reciprocal_GrandTotal_RGT]
       ,[MAX_RGT]
      ,[MIN_RGT]
      ,[Year_of_WP]
     
      ,[Recalibrated_Scaled_Index]
    
      ,[Weight]
      ,[Weighted_Score]
      ,[Consession_Type]
  FROM [dbo].[WP_OML_ACQUSITION_INDEX_WEIGHTED_SCORE]
  
  UNION ALL
  
  --SELECT  [CompanyName]
  --,[INDEX_TYPE]
  --    ,[Unscaled_Score]
  --    ,[Unscaled_Score_sum]
  --    ,[Scaled_by_Reciprocal_GrandTotal_RGT]
  --     ,[MAX_RGT]
  --    ,[MIN_RGT]
  --    ,[Year_of_WP]
     
  --    ,[Recalibrated_Scaled_Index]
    
  --    ,[Weight]
  --    ,[Weighted_Score]
  --    ,[Consession_Type]
  --FROM [WP_OML_COMPLIANCE_INDEX_WEIGHTED_SCORE]
  
  -- UNION ALL
  
  SELECT  [CompanyName]
  ,[INDEX_TYPE]
      ,[Unscaled_Score]
      ,[Unscaled_Score_sum]
      ,[Scaled_by_Reciprocal_GrandTotal_RGT]
       ,[MAX_RGT]
      ,[MIN_RGT]
      ,[Year]
     
      ,[Recalibrated_Scaled_Index]
    
      ,[Weight]
      ,[Weighted_Score]
      ,[Consession_Type]
  FROM [dbo].WP_OML_CONCESSION_RENTALS_INDEX_WEIGHTED_SCORE
  
   UNION ALL
  
  SELECT  [CompanyName]
  ,[INDEX_TYPE]
      ,[Unscaled_Score]
      ,[Unscaled_Score_sum]
      ,[Scaled_by_Reciprocal_GrandTotal_RGT]
       ,[MAX_RGT]
      ,[MIN_RGT]
      ,[Year_of_WP]
     
      ,[Recalibrated_Scaled_Index]
    
      ,[Weight]
      ,[Weighted_Score]
      ,[Consession_Type]
  FROM [dbo].WP_OML_DISCOVERY_INDEX_WEIGHTED_SCORE
  
   UNION ALL
  
  SELECT  [CompanyName]
  ,[INDEX_TYPE]
      ,[Unscaled_Score]
      ,[Unscaled_Score_sum]
      ,[Scaled_by_Reciprocal_GrandTotal_RGT]
       ,[MAX_RGT]
      ,[MIN_RGT]
      ,[Year_of_WP]
     
      ,[Recalibrated_Scaled_Index]
    
      ,[Weight]
      ,[Weighted_Score]
      ,[Consession_Type]
  FROM [dbo].WP_OML_EXPLORATORY_DRILLING_INDEX_WEIGHTED_SCORE
