namespace Backend_UMR_Work_Program.DataModels
{
	public class ProcessStatus
	{
		public int Id { get; set; }
		public string? StatusName { get; set; }
		public string? CreatedBy { get; set; }
		public DateTime CreateOn { get; set; }
		public string? UpdatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}
