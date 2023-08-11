namespace Bloggie.Web.Models.ViewModels
{
    public record AddTagRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
