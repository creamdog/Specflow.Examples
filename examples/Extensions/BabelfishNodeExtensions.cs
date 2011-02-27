using System;
using System.IO;
using System.Linq;
using System.Text;
using Babelfish;

namespace service.tests.Extensions
{
    public static class BabelfishNodeExtensions
    {
        public static void AssertMemberEquality(this INode self, INode that)
        {
            if (self.Name.Trim() != that.Name.Trim())
                throw new ApplicationException(string.Format("name missmatch: expected {0} but got {1}", self.Name.Trim(), that.Name.Trim()));

            if (self.Text.Trim() != that.Text.Trim())
                throw new ApplicationException(string.Format("value missmatch: expected {0} but got {1}", self.Text.Trim(), that.Text.Trim()));

            for(var i = 0; i < self.ChildNodes.Count; i++)
            {
                var node = self.ChildNodes[i];

                var thatMatch = that.ChildNodes.Where(n => n.Name == node.Name).ToList();

                if (thatMatch.Count == 0)
                    throw new ApplicationException(string.Format("member '{0}' not found in {1}", node.Name, that.Name));

                for (var j = 0; j < thatMatch.Count(); j++)
                {
                    var node2 = thatMatch[j];

                    try
                    {
                        node.AssertMemberEquality(node2);
                        break;
                    }
                    catch (ApplicationException)
                    {
                        if (j == thatMatch.Count - 1)
                            throw;
                    }
                }
            }
        }

        public static string ToXmlString(this INode node)
        {
            using(var ms = new MemoryStream())
            {
                Babelfish.Converters.XmlConverter.ConvertToXmlDocument(node).Save(ms);
                return Encoding.UTF8.GetString(ms.GetBuffer());
            }
        }
    }
}
