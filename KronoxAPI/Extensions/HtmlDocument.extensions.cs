using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Extensions;

public static class HtmlDocumentExtensions
{
    public static bool SessionExpired(this HtmlDocument doc)
    {
        return doc.DocumentNode.SelectSingleNode("//form[@id=\"loginform\"]") != null;
    }

}
