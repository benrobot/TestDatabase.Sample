using System.Collections.Generic;

namespace TestDatabase.Sample.WebApiWithEFCore.ApiModels
{
    public class BlogSummary
    {
        public string Url { get; set; }
        public int PostCount { get; set; }
        public List<PostTitle> PostTitles { get; set; }
    }
}