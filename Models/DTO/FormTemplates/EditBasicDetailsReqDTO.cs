namespace LightMethods.Survey.Models.DTO
{
    public class EditBasicDetailsReqDTO
    {
        public string Code { get; set; }

        public string Title { get; set; }

        public double Version { get; set; }

        public string Description { set; get; }

        public string Colour { get; set; }
    }
}
