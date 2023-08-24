using System.Text;
using System.Text.RegularExpressions;
using Markdig;

namespace GuidanceExplorer.Indexer;

public static class MarkdownSectionParser
{
    private static Regex SeparatorPattern = new Regex("^-+$");
    private static Regex HeadingPattern = new Regex("^#+\\s*(.+)$");

    public static List<MarkdownSection> Parse(StreamReader reader, string fileName)
    {
        var atStartOfFile = true;
        var readingYamlProperties = false;

        var propertiesContentBuilder = new StringBuilder();
        var markdownSectionContentBuilder = new StringBuilder();
        var sectionTitle = "";
        var sections = new List<MarkdownSection>();
        var sectionIndex = 1;

        while (reader.ReadLine() is { } line)
        {
            // YAML properties are separated from the rest of the content by a line that has three or more dashes.
            // This parser code takes care of splitting out the YAML from the rest of the markdown. 
            if (!atStartOfFile && readingYamlProperties && SeparatorPattern.Match(line).Success)
            {
                readingYamlProperties = false;
            }
            
            if (atStartOfFile && SeparatorPattern.IsMatch(line))
            {
                atStartOfFile = false;
                readingYamlProperties = true;
            }

            // We could do something with the YAML properties later, that's why we're saving them.
            // If we're not reading properties, we need to process sections.
            // Each section has a heading. The first section in the file doesn't need a section.
            if (readingYamlProperties)
            {
                propertiesContentBuilder.AppendLine(line);
            }
            else
            {
                var headingPatternMatch = HeadingPattern.Match(line);

                if (headingPatternMatch.Success)
                {
                    var sectionContent = markdownSectionContentBuilder.ToString();

                    if (!string.IsNullOrEmpty(sectionContent.Trim()))
                    {
                        var section = new MarkdownSection()
                        {
                            Id = $"{fileName}-{sectionIndex}",
                            FileName = fileName,
                            Title = sectionTitle,
                            Content = Markdown.ToPlainText(markdownSectionContentBuilder.ToString())
                        };

                        sections.Add(section);
                    }

                    markdownSectionContentBuilder = new StringBuilder();
                    sectionTitle = headingPatternMatch.Groups[1].Value;
                    sectionIndex++;
                }

                markdownSectionContentBuilder.AppendLine(line);
            }
        }
        
        // If we're at the end of the file, we should finish up reading the final section.
        // Otherwise you're missing out on a bunch of content.
        sections.Add(new MarkdownSection()
        {
            Id = fileName,
            FileName = fileName,
            Title = sectionTitle,
            Content = Markdown.ToPlainText(markdownSectionContentBuilder.ToString())
        });

        return sections;
    }
}