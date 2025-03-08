using System.Text.RegularExpressions;

namespace sts_ai_support.Models
{
    public class HtmlTag
    {
        public string TagType { get; set; }
        public string TagValue { get; set; }

        public HtmlTag(GroupCollection groups)
        {
            TagType = groups.Values.Skip(1).First().Value.Trim();
            TagValue = groups.Values.Skip(2).First().Value.Trim();
        }
    }
}
