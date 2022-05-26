
using Backend_UMR_Work_Program.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Backend_UMR_Work_Program.Models.GeneralModel;

namespace Backend_UMR_Work_Program.Helper.AutoMapperSettings
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
        
            CreateMap<UserMaster, UserMasterModel>().ReverseMap();
            CreateMap<CONCESSION_SITUATION, CONCESSION_SITUATION_Model>().ReverseMap();
            CreateMap<GEOPHYSICAL_ACTIVITIES_ACQUISITION, GEOPHYSICAL_ACTIVITIES_ACQUISITION_Model>().ReverseMap();
            CreateMap<GEOPHYSICAL_ACTIVITIES_PROCESSING, GEOPHYSICAL_ACTIVITIES_PROCESSING_Model>().ReverseMap();
            CreateMap<DRILLING_OPERATIONS_CATEGORIES_OF_WELL, DRILLING_OPERATIONS_CATEGORIES_OF_WELL_Model>().ReverseMap();
            CreateMap<DRILLING_EACH_WELL_COST, DRILLING_EACH_WELL_COST_Model>().ReverseMap();
            CreateMap<DRILLING_EACH_WELL_COST_PROPOSED, DRILLING_EACH_WELL_COST_PROPOSED_Model>().ReverseMap();
        }
    }
}
