namespace Notes.Web.IntegrationServices
{
    public class NotesServiceConfiguration
    {
        public string Url { get; set; }
        public class Methods
        {
            public const string Get = "Note";
            public const string Create = "Note";
            public const string Delete = "Note?id={0}";
            public const string GetById = "Note/GetById";
        }
    }
}
