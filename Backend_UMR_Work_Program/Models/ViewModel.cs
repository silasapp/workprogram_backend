namespace Backend_UMR_Work_Program.Models
{
    public class ViewModel
    {
        public class UserToken
        {
            public long? id { get; set; }
            public string? token { get; set; }
            public int code { get; set; }
            public string? pass { get; set; }
        }
    }
}
