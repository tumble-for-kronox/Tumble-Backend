using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using KronoxAPI.Model.Scheduling;
using HtmlAgilityPack;

namespace KronoxAPI.Parser;

/// <summary>
/// Manages all parsing of search results from Kronox as well as related methods.
/// </summary>
public class SearchParser
{
    /// <summary>
    /// Parse search results given in Kronox's standard HTML endpoint response (not page grab) format.
    /// </summary>
    /// <param name="htmlSearchResults"></param>
    /// <returns>The corresponding objects <see cref="List{Programme}"/> to the search results found in <paramref name="htmlSearchResults"/>.
    /// Any value within the <see cref="Programme"/> objects that could not be parsed correctly will output as <see cref="string"/> "N/A".</returns>
    public static List<Programme> ParseToProgrammes(string htmlSearchResults)
    {
        HtmlDocument document = new();
        document.LoadHtml(htmlSearchResults);

        List<Programme> foundProgrammes = new();

        // Get all links in the return html (as per Kronox's standard). Skip the first two as they link to non-programme entities
        IEnumerable<HtmlNode> hyperLinksInPage = document.DocumentNode.Descendants("a").Skip(2);

        foreach (HtmlNode hyperlink in hyperLinksInPage)
        {
            if (hyperlink.GetAttributeValue("target", "") != "_blank") continue;

            // Get href value from link
            string hyperlinkHref = hyperlink.GetAttributeValue("href", "N/A");
            string id;

            // If the link can't be found we can' use the entry, hence we don't wish to continue parsing it
            if (hyperlinkHref == "N/A") continue;

            // Match the part that contains the id
            Match idMatch = Regex.Match(hyperlinkHref, "resurser=(?<scheduleId>.*)$");
            // Make sure the regex group exists
            idMatch.Groups.TryGetValue("scheduleId", out Group? idGroup);

            // If the ID can't be matched we can't use the entry, hence we move on
            if (idGroup == null) continue;
            
            id = idGroup.Value;

            string[] splitTitles = hyperlink.InnerText.Split(",");

            string title = splitTitles[0];
            string subtitle = RemoveDuplicateWords(splitTitles[1]);

            foundProgrammes.Add(new Programme(title, subtitle, id));
        }

        return foundProgrammes;
    }

    /// <summary>
    /// <para>
    /// Removes duplicate words from the given string.
    /// </para>
    /// <para>
    /// Source code found:
    /// https://stackoverflow.com/a/1058850/18690430
    /// </para>
    /// </summary>
    /// <param name="stringWithDuplicates"></param>
    /// <returns></returns>
    public static string RemoveDuplicateWords(string stringWithDuplicates)
    {
        var words = new HashSet<string>();
        return Regex.Replace(stringWithDuplicates, "\\w+", m =>
                             words.Add(m.Value.ToUpperInvariant())
                                 ? m.Value
                                 : String.Empty);
    }

}
