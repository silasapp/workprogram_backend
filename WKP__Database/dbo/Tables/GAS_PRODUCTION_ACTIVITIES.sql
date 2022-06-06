﻿CREATE TABLE [dbo].[GAS_PRODUCTION_ACTIVITIES] (
    [Id]                                                                                                  INT           IDENTITY (1, 1) NOT NULL,
    [OML_ID]                                                                                              VARCHAR (200) NULL,
    [OML_Name]                                                                                            VARCHAR (200) NULL,
    [CompanyName]                                                                                         VARCHAR (500) NULL,
    [Companyemail]                                                                                        VARCHAR (500) NULL,
    [Year_of_WP]                                                                                          VARCHAR (100) NULL,
    [Current_Actual_Year]                                                                                 VARCHAR (500) NULL,
    [Utilized]                                                                                            VARCHAR (500) NULL,
    [Flared]                                                                                              VARCHAR (500) NULL,
    [Forecast_]                                                                                           VARCHAR (500) NULL,
    [Remarks_]                                                                                            VARCHAR (MAX) NULL,
    [How_many_NAG_fields_have_approved_Gas_FDP]                                                           VARCHAR (500) NULL,
    [Gas_wells_drilled_and_planned]                                                                       VARCHAR (500) NULL,
    [Gas_production_and_flare_historical_performance]                                                     VARCHAR (500) NULL,
    [Gas_plant_capacity]                                                                                  VARCHAR (500) NULL,
    [Ongoing_and_planned_Gas_plant_projects]                                                              VARCHAR (500) NULL,
    [Domestic_gas_obligation]                                                                             VARCHAR (500) NULL,
    [Planned_projects_for_domestic_supply]                                                                VARCHAR (500) NULL,
    [Gas_Field_Development_Plan_Approval]                                                                 VARCHAR (500) NULL,
    [Gas_wells_drilled_and_wells_planned]                                                                 VARCHAR (500) NULL,
    [AG_NAG_and_Condensate_in_place_volumes_and_Reserves_Reserves_for_reservoirs_and_Fields]              VARCHAR (500) NULL,
    [Maturation_plans_for_leads_and_prospects_leading_to_reserves_growth]                                 VARCHAR (500) NULL,
    [Current_production_status_for_all_Gas_and_condensate_Reservoirs]                                     VARCHAR (500) NULL,
    [Current_gas_production_utilisation_and_Flare_volumes]                                                VARCHAR (500) NULL,
    [Sources_of_Gas_utilisation_should_be_clearly_stated]                                                 VARCHAR (500) NULL,
    [Gas_Production_and_flare_historical_Performance_5_years_Performance_and_Plan]                        VARCHAR (500) NULL,
    [Substantiate_flare_reduction_methods_with_projects]                                                  VARCHAR (500) NULL,
    [Annotate_clearly_reasons_for_increase_or_reduction_in_Gas_production_utilisation_and_flare_profiles] VARCHAR (500) NULL,
    [Current_pressures_for_Gas_and_condensate_Reservoirs]                                                 VARCHAR (500) NULL,
    [Production_forecast_for_all_Gas_and_condensate_reservoirs]                                           VARCHAR (500) NULL,
    [Gas_compositional_Analysis]                                                                          VARCHAR (500) NULL,
    [Feed_gas_Volumes_into_the_Processing_Facility]                                                       VARCHAR (500) NULL,
    [Gas_Plant_Capacity_NEW]                                                                              VARCHAR (500) NULL,
    [Plant_Utilization_Capacity]                                                                          VARCHAR (500) NULL,
    [Plant_maintenance_activities]                                                                        VARCHAR (500) NULL,
    [ongoing_and_planned_Gas_plant_projects_NEW]                                                          VARCHAR (500) NULL,
    [LNG_and_NGLs_Production_forecast]                                                                    VARCHAR (500) NULL,
    [Custody_Transfer_status]                                                                             VARCHAR (500) NULL,
    [License_Renewal_Status_for_all_Gas_plants]                                                           VARCHAR (500) NULL,
    [Pipeline_license_renewal_staus]                                                                      VARCHAR (500) NULL,
    [Domestic_Gas_Supply_DSO]                                                                             VARCHAR (500) NULL,
    [Projects_planned_for_Domestic_supply_Gas_to_power_industries_etc]                                    VARCHAR (500) NULL,
    [Actual_year]                                                                                         VARCHAR (300) NULL,
    [proposed_year]                                                                                       VARCHAR (300) NULL,
    [Created_by]                                                                                          VARCHAR (100) NULL,
    [Updated_by]                                                                                          VARCHAR (100) NULL,
    [Date_Created]                                                                                        DATETIME      NULL,
    [Date_Updated]                                                                                        DATETIME      NULL,
    [Contract_Type]                                                                                       VARCHAR (50)  NULL,
    [Terrain]                                                                                             VARCHAR (50)  NULL,
    [penaltyfeepaid]                                                                                      VARCHAR (50)  NULL,
    [Amount_Paid]                                                                                         VARCHAR (50)  NULL,
    [Consession_Type]                                                                                     VARCHAR (50)  NULL,
    [proposed_production]                                                                                 VARCHAR (50)  NULL,
    [proposed_utilization]                                                                                VARCHAR (50)  NULL,
    [proposed_flaring]                                                                                    VARCHAR (50)  NULL,
    [Gas_flare_Royalty_payment]                                                                           VARCHAR (50)  NULL,
    [Gas_Sales_Royalty_Payment]                                                                           VARCHAR (50)  NULL,
    [No_of_gas_well_planned]                                                                              VARCHAR (50)  NULL,
    [No_of_gas_well_drilled]                                                                              VARCHAR (50)  NULL,
    [Is_there_a_gas_plant]                                                                                VARCHAR (50)  NULL,
    [No_of_ongoing_projects]                                                                              VARCHAR (50)  NULL,
    [No_of_plannned_projects]                                                                             VARCHAR (50)  NULL,
    [Is_there_a_license_to_operate_a_gas_plant]                                                           VARCHAR (50)  NULL,
    [Domestic_Gas_obligation_met]                                                                         VARCHAR (50)  NULL,
    [Has_annual_NDR_subscription_fee_been_paid]                                                           VARCHAR (50)  NULL,
    [When_was_the_date_of_your_last_NDR_submission]                                                       DATE          NULL,
    [Upload_NDR_payment_receipt]                                                                          VARCHAR (200) NULL,
    [Are_you_up_to_date_with_your_NDR_data_submission]                                                    VARCHAR (20)  NULL,
    [NDRFilename]                                                                                         VARCHAR (500) NULL,
    [number_of_gas_wells_completed]                                                                       VARCHAR (100) NULL,
    [number_of_gas_wells_tested]                                                                          VARCHAR (100) NULL,
    [COMPANY_ID]                                                                                          VARCHAR (100) NULL,
    [CompanyNumber]                                                                                       INT           NULL,
    CONSTRAINT [PK_GAS_PRODUCTION_ACTIVITIES] PRIMARY KEY CLUSTERED ([Id] ASC)
);

