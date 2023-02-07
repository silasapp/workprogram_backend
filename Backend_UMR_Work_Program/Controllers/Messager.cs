using Microsoft.AspNetCore.Authentication.JwtBearer;
using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Security.Claims;
using static Backend_UMR_Work_Program.Models.GeneralModel;
namespace Backend_UMR_Work_Program.Controllers
{
    public static class Messager
    {
        public static string ShowMessage(string action)
        {
            var message = "";
            if (GeneralModel.Insert == "Insert")
            {
                message = action + "ED";
            }
            else
            {
                message = action + "D";
            }
            return $"{message}  successfully.";
        }
    }
}
