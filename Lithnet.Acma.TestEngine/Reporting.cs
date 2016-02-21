namespace Lithnet.Acma.TestEngine
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Drawing;

    /// <summary>
    /// An item used in generating reports that has a title and one or more value elements
    /// </summary>
    public class ReportingItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingItem" /> struct
        /// </summary>
        public ReportingItem()
        {
            this.ReportingItemValues = new List<ReportingCell>();
            this.TitleBackgroundColor = ColorTranslator.FromHtml("#EEEEEE");
            this.TitleTextColor = Color.Black;
        }

        /// <summary>
        /// The color of the title cell
        /// </summary>
        public Color TitleBackgroundColor { get; set; }

        /// <summary>
        /// The color of the text in the title cell
        /// </summary>
        public Color TitleTextColor { get; set; }

        /// <summary>
        /// Gets or sets the title of the reporting item
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the values associated with this reporting item
        /// </summary>
        public List<ReportingCell> ReportingItemValues { get; private set; }

        /// <summary>
        /// Adds a new reporting item value
        /// </summary>
        /// <param name="value">The value to add</param>
        public void AddValue(string value)
        {
            ReportingCell reportingItemValue = new ReportingCell(value);
            this.ReportingItemValues.Add(reportingItemValue);
        }

        /// <summary>
        /// Adds a new reporting item value and specifies the text color
        /// </summary>
        /// <param name="value">The value to add</param>
        /// <param name="textColor">The color of the text</param>
        public void AddValue(string value, Color textColor)
        {
            ReportingCell reportingItemValue = new ReportingCell(value, textColor);
            this.ReportingItemValues.Add(reportingItemValue);
        }

        /// <summary>
        /// Adds a new reporting item value and specifies the text and background colors
        /// </summary>
        /// <param name="value">The value to add</param>
        /// <param name="textColor">The color of the text</param>
        /// <param name="backgroundColor">The color of the cell background</param>
        public void AddValue(string value, Color textColor, Color backgroundColor)
        {
            ReportingCell reportingItemValue = new ReportingCell(value, textColor, backgroundColor);
            this.ReportingItemValues.Add(reportingItemValue);
        }
    }

    public class ReportingCell
    {
        public ReportingCell()
        {
            this.ValueBackgroundColor = ColorTranslator.FromHtml("#EEEEEE");
            this.ValueTextColor = Color.Black;
        }

        public ReportingCell(string value)
        {
            this.Value = value;
            this.ValueBackgroundColor = ColorTranslator.FromHtml("#EEEEEE");
            this.ValueTextColor = Color.Black;
        }

        public ReportingCell(string value, Color textColor)
        {
            this.Value = value;
            this.ValueTextColor = textColor;
            this.ValueBackgroundColor = ColorTranslator.FromHtml("#EEEEEE");
        }

        public ReportingCell(string value, Color textColor, Color backgroundColor)
        {
            this.Value = value;
            this.ValueTextColor = textColor;
            this.ValueBackgroundColor = backgroundColor;
        }

        public Color ValueBackgroundColor { get; set; }

        public Color ValueTextColor { get; set; }

        public string Value { get; set; }
    }

    public class ColumnHeader : ReportingCell
    {
        public ColumnHeader() :
            base()
        {
        }

        public ColumnHeader(string value)
            : base(value)
        {
        }

        public ColumnHeader(string value, int columnWidthPx)
            : base(value)
        {
            this.ColumnWidthPx = columnWidthPx;
        }

        public ColumnHeader(string value, decimal columnWidthPercent)
            : base(value)
        {
            this.ColumnWidthPercent = columnWidthPercent;
        }

        public ColumnHeader(string value, Color textColor)
            : base(value, textColor)
        {
        }

        public ColumnHeader(string value, Color textColor, int columnWidthPx)
            : base(value, textColor)
        {
            this.ColumnWidthPx = columnWidthPx;
        }

        public ColumnHeader(string value, Color textColor, decimal columnWidthPercent)
            : base(value, textColor)
        {
            this.ColumnWidthPercent = columnWidthPercent;
        }

        public ColumnHeader(string value, Color textColor, Color backgroundColor)
            : base(value, textColor, backgroundColor)
        {
        }

        public ColumnHeader(string value, Color textColor, Color backgroundColor, int columnWidthPx)
            : base(value, textColor, backgroundColor)
        {
            this.ColumnWidthPx = columnWidthPx;
        }

        public ColumnHeader(string value, Color textColor, Color backgroundColor, decimal columnWidthPercent)
            : base(value, textColor, backgroundColor)
        {
            this.ColumnWidthPercent = columnWidthPercent;
        }

        public decimal ColumnWidthPx { get; set; }

        public decimal ColumnWidthPercent { get; set; }
    }

    public abstract class Section
    {
        public Section()
        {
            this.SectionTitleBackgroundColor = Color.Black;
            this.SectionTitleTextColor = Color.White;
        }

        public string SectionTitle { get; set; }
        public Color SectionTitleTextColor { get; set; }
        public Color SectionTitleBackgroundColor { get; set; }
        public int SectionTitleFontSize { get; set; }
    }

    public class ReportingItemSection : Section
    {
        public bool AutoEqualColumnWidth { get; set; }
        public int IndentLevel { get; set; }

        public ReportingItemSection()
            : base()
        {
            this.AutoEqualColumnWidth = false;
            this.ColumnHeadings = new List<ColumnHeader>();
            this.ReportingItems = new List<ReportingItem>();
        }

        public List<ColumnHeader> ColumnHeadings { get; private set; }

        public List<ReportingItem> ReportingItems { get; private set; }

        public ColumnHeader AddHeading(string value)
        {
            return this.AddHeading(value, Color.Empty, Color.Empty);
        }

        public ColumnHeader AddHeading(string value, int columnWidthPercent)
        {
            return this.AddHeading(value, Color.Empty, Color.Empty, columnWidthPercent);
        }

        public ColumnHeader AddHeading(string value, decimal columnWidthPx)
        {
            return this.AddHeading(value, Color.Empty, Color.Empty, columnWidthPx);
        }

        public ColumnHeader AddHeading(string value, Color textColor)
        {
            return this.AddHeading(value, textColor, Color.Empty, 0);
        }

        public ColumnHeader AddHeading(string value, Color textColor, decimal columnWidthPercent)
        {
            return this.AddHeading(value, textColor, Color.Empty, columnWidthPercent);
        }

        public ColumnHeader AddHeading(string value, Color textColor, int columnWidthPx)
        {
            return this.AddHeading(value, textColor, Color.Empty, columnWidthPx);
        }

        public ColumnHeader AddHeading(string value, Color textColor, Color backgroundColor)
        {
            return this.AddHeading(value, textColor, backgroundColor, 0);
        }

        public ColumnHeader AddHeading(string value, Color textColor, Color backgroundColor, decimal columnWidthPercent)
        {
            ColumnHeader cell = new ColumnHeader(value, textColor, backgroundColor);
            cell.ColumnWidthPercent = columnWidthPercent;
            this.ColumnHeadings.Add(cell);
            return cell;
        }

        public ColumnHeader AddHeading(string value, Color textColor, Color backgroundColor, int columnWidthPx)
        {
            ColumnHeader cell = new ColumnHeader(value, textColor, backgroundColor);
            cell.ColumnWidthPx = columnWidthPx;
            this.ColumnHeadings.Add(cell);
            return cell;
        }

        public ReportingItem AddReportingItem(string title, string value)
        {
            ReportingItem item = new ReportingItem();
            item.Title = title;
            item.AddValue(value);
            this.ReportingItems.Add(item);
            return item;
        }

        public ReportingItem AddReportingItem(string title, string value1, string value2)
        {
            ReportingItem item = new ReportingItem();
            item.Title = title;
            item.AddValue(value1);
            item.AddValue(value2);
            this.ReportingItems.Add(item);
            return item;
        }

        public ReportingItem AddReportingItem(string title, string value1, string value2, string value3)
        {
            ReportingItem item = new ReportingItem();
            item.Title = title;
            item.AddValue(value1);
            item.AddValue(value2);
            item.AddValue(value3);
            this.ReportingItems.Add(item);
            return item;
        }
    }

    public class TextSection : Section
    {
        public TextSection()
            : base()
        {
        }

        public string Text { get; set; }
    }

    public class Report
    {
        public Report()
        {
            this.ReportHeading = new ReportingCell();
            this.Sections = new List<Section>();
            this.Attachments = new Dictionary<string, byte[]>();
        }

        public string ReportSubject { get; set; }
        public List<Section> Sections { get; set; }
        public ReportingCell ReportHeading { get; set; }
        public ReportingCell ReportSubHeading { get; set; }
        public string VersionText { get; set; }
        public Dictionary<string, byte[]> Attachments { get; private set; }

        public void AddSection(Section section)
        {
            this.Sections.Add(section);
        }

        public void AddAttachment(string name, byte[] data)
        {
            this.Attachments.Add(name, data);
        }

        public string GenerateHtmlReport()
        {
            StringBuilder report = new StringBuilder();

            report.Append("<html>");

            report.Append(GenerateHeading());
            report.Append(GenerateSections());

            report.Append("</html>");
            return report.ToString();
        }

        private string GenerateHeading()
        {
            StringBuilder heading = new StringBuilder();

            heading.Append("<font face=\"Calibri\" size=\"5\" color=\"" + ColorTranslator.ToHtml(this.ReportHeading.ValueTextColor)
                     + "\">" + CleanHtml(this.ReportHeading.Value) + "</font><br/>");

            if (this.ReportSubHeading != null)
            {
                heading.Append("<font face=\"Calibri\" size=\"4\" color=\"" + ColorTranslator.ToHtml(this.ReportSubHeading.ValueTextColor)
                    + "\">" + CleanHtml(this.ReportSubHeading.Value) + "</font><br/>");
            }

            heading.Append("<font face=\"Calibri\" size=\"1\"><p align=\"right\">" + CleanHtml(this.VersionText) + "</font></p>");



            return heading.ToString();
        }

        private string GenerateSections()
        {
            StringBuilder report = new StringBuilder();

            foreach (Section section in this.Sections)
            {
                if (section.GetType() == typeof(TextSection))
                {
                    report.AppendLine(GenerateTextSection((TextSection)section));
                }
                else if (section.GetType() == typeof(ReportingItemSection))
                {
                    report.AppendLine(GenerateReportingItemSection((ReportingItemSection)section));
                }
            }

            return report.ToString();
        }

        private string GenerateTextSection(TextSection section)
        {
            StringBuilder report = new StringBuilder();
            string fontsize = section.SectionTitleFontSize == 0 ? "3" : section.SectionTitleFontSize.ToString();

            string headingRowHtml = "<table border=\"0\" width=\"100%\" cellpadding=\"1\" cellspacing=\"0\">" +
                                    "<tr><td bgcolor=\"" + ColorTranslator.ToHtml(section.SectionTitleBackgroundColor) + "\" colspan=\"1\">" +
                                    "<font face=\"Calibri\" size=\"" + fontsize + "\" color=\"" + ColorTranslator.ToHtml(section.SectionTitleTextColor) + "\">" +
                                    "<b>" + CleanHtml(section.SectionTitle) + "</b></font></td></tr></table>";

            report.AppendLine(headingRowHtml);
            report.AppendLine("<pre>");
            report.AppendLine(CleanHtml(section.Text, false));
            report.AppendLine("</pre");

            return report.ToString();
        }

        private string GenerateReportingItemSection(ReportingItemSection section)
        {
            StringBuilder report = new StringBuilder();

            string cellDefinitionHtml = "<td bgcolor=\"<!--BGCOLOR-->\"><font face=\"Calibri\" size=\"2\" color=\"<!--TEXTCOLOR-->\"><!--VALUE--></font></td>";
            string headingCellDefinitionHtml = "<td bgcolor=\"<!--BGCOLOR-->\" <!--WIDTH-->><font face=\"Calibri\" size=\"2\" color=\"<!--TEXTCOLOR-->\"><b><!--VALUE--></b></font></td>";
            string fontsize = section.SectionTitleFontSize == 0 ? "3" : section.SectionTitleFontSize.ToString();

            string headingRowHtml = "<table border=\"0\" width=\"100%\" cellpadding=\"1\" cellspacing=\"0\">" +
                                    "<tr><!--INDENTCELLS--><td bgcolor=\"" + ColorTranslator.ToHtml(section.SectionTitleBackgroundColor) + "\" colspan=\"1\">" +
                                    "<font face=\"Calibri\" size=\"" + fontsize + "\" color=\"" + ColorTranslator.ToHtml(section.SectionTitleTextColor) + "\">" +
                                    "<b>" + CleanHtml(section.SectionTitle) + "</b></font></td></tr></table>";


            for (int x = 0; x < section.IndentLevel; x++)
            {
                headingRowHtml = headingRowHtml.Replace("<!--INDENTCELLS-->", "<td width=\"15\"></td><!--INDENTCELLS-->");
            }

            headingRowHtml = headingRowHtml.Replace("<!--INDENTCELLS-->", string.Empty);

            report.AppendLine(headingRowHtml);
            report.AppendLine("<table width=\"100%\">");

            if (section.ColumnHeadings.Count > 0)
            {
                report.AppendLine("<tr>");

                for (int x = 0; x < section.IndentLevel; x++)
                {
                    report.AppendLine("<td width=\"15\"></td>");
                }

                foreach (ColumnHeader cell in section.ColumnHeadings)
                {
                    string width = string.Empty;

                    if (section.AutoEqualColumnWidth)
                    {
                        decimal percent = ((decimal)1 / section.ColumnHeadings.Count) * 100;
                        width = string.Format("{0}", percent.ToString("F2") + "%");
                    }
                    else
                    {
                        if (cell.ColumnWidthPx != 0)
                        {
                            width = string.Format("{0}", cell.ColumnWidthPx);
                        }
                        else if (cell.ColumnWidthPercent != 0)
                        {
                            width = string.Format("{0}", cell.ColumnWidthPercent.ToString("F2") + "%");
                        }
                    }

                    string cellHtml = headingCellDefinitionHtml.Replace("<!--BGCOLOR-->", ColorTranslator.ToHtml(cell.ValueBackgroundColor));
                    cellHtml = cellHtml.Replace("<!--TEXTCOLOR-->", ColorTranslator.ToHtml(cell.ValueTextColor));
                    cellHtml = cellHtml.Replace("<!--VALUE-->", CleanHtml(cell.Value));
                    cellHtml = cellHtml.Replace("<!--WIDTH-->", "width=\"" + width + "\"");
                    report.AppendLine(cellHtml);
                }
                report.AppendLine("</tr>");
            }
            else
            {
                cellDefinitionHtml = cellDefinitionHtml.Replace("<!--WIDTH-->", "\"100%\"");
            }


            foreach (ReportingItem item in section.ReportingItems)
            {
                report.AppendLine("<tr>");

                for (int x = 0; x < section.IndentLevel; x++)
                {
                    report.AppendLine("<td width=\"15\"></td>");
                }

                string titleCellHtml = cellDefinitionHtml.Replace("<!--BGCOLOR-->", ColorTranslator.ToHtml(item.TitleBackgroundColor));
                titleCellHtml = titleCellHtml.Replace("<!--TEXTCOLOR-->", ColorTranslator.ToHtml(item.TitleTextColor));
                titleCellHtml = titleCellHtml.Replace("<!--VALUE-->", CleanHtml(item.Title));
                report.AppendLine(titleCellHtml);

                foreach (ReportingCell value in item.ReportingItemValues)
                {
                    string cellHtml = cellDefinitionHtml.Replace("<!--BGCOLOR-->", ColorTranslator.ToHtml(value.ValueBackgroundColor));
                    cellHtml = cellHtml.Replace("<!--TEXTCOLOR-->", ColorTranslator.ToHtml(value.ValueTextColor));
                    cellHtml = cellHtml.Replace("<!--VALUE-->", CleanHtml(value.Value));
                    report.AppendLine(cellHtml);
                }

                // Add in any blank cells required to make a full row
                if (item.ReportingItemValues.Count < section.ColumnHeadings.Count - 1)
                {
                    int additionalCellsRequired = section.ColumnHeadings.Count - item.ReportingItemValues.Count;
                    ReportingCell emptyCell = new ReportingCell();

                    for (int x = 0; x < additionalCellsRequired; x++)
                    {
                        string cellHtml = cellDefinitionHtml.Replace("<!--BGCOLOR-->", ColorTranslator.ToHtml(emptyCell.ValueBackgroundColor));
                        cellHtml = cellHtml.Replace("<!--TEXTCOLOR-->", ColorTranslator.ToHtml(emptyCell.ValueTextColor));
                        cellHtml = cellHtml.Replace("<!--VALUE-->", CleanHtml(emptyCell.Value));
                        report.AppendLine(cellHtml);
                    }
                }

                report.AppendLine("</tr>");
            }

            report.AppendLine("</table>");
            report.AppendLine("<br>");

            return report.ToString();
        }

        private static string CleanHtml(string htmlText, bool replaceLineBreaks = true)
        {
            if (htmlText == null)
            {
                return null;
            }

            bool needEncode = false;
            for (int i = 0; i < htmlText.Length; i++)
            {
                char c = htmlText[i];
                if (c == '&' || c == '"' || c == '<' || c == '>' || c > 159 || c == '\n')
                {
                    needEncode = true;
                    break;
                }
            }

            if (!needEncode)
            {
                return htmlText;
            }

            StringBuilder output = new StringBuilder();

            int len = htmlText.Length;

            for (int i = 0; i < len; i++)
            {
                switch (htmlText[i])
                {
                    case '\n':
                        if (replaceLineBreaks)
                        {
                            output.Append("<br>");
                        }
                        break;
                    case '&':
                        output.Append("&amp;");
                        break;
                    case '>':
                        output.Append("&gt;");
                        break;
                    case '<':
                        output.Append("&lt;");
                        break;
                    case '"':
                        output.Append("&quot;");
                        break;
                    default:
                        // MS starts encoding with &# from 160 and stops at 255.
                        // We don't do that. One reason is the 65308/65310 unicode
                        // characters that look like '<' and '>'.

                        if (htmlText[i] > 159)
                        {
                            output.Append("&#");
                            output.Append(((int)htmlText[i]).ToString());
                            output.Append(";");
                        }
                        else
                        {
                            output.Append(htmlText[i]);
                        }
                        break;
                }
            }
            return output.ToString();
        }

        private static void TestReport()
        {
            Report report = new Report();

            report.VersionText = "my report 1.1.1.1";
            report.ReportHeading = new ReportingCell("My test report", System.Drawing.Color.Red);

            ReportingItemSection section = new ReportingItemSection();

            section.SectionTitle = "First section";
            section.SectionTitleBackgroundColor = System.Drawing.Color.Black;
            section.SectionTitleTextColor = System.Drawing.Color.White;
            section.AddHeading("Item");
            section.AddHeading("Value1");
            section.AddHeading("Value2");
            section.AddReportingItem("my item1", "value1", "value5");
            section.AddReportingItem("my item2", "value2", "value6");
            section.AddReportingItem("my item3", "value3", "value7");
            section.AddReportingItem("my item4", "value4", "value8");
            report.AddSection(section);

            section = new ReportingItemSection();
            section.SectionTitle = "Second section";
            section.SectionTitleBackgroundColor = System.Drawing.Color.Red;
            section.SectionTitleTextColor = System.Drawing.Color.White;
            section.AddHeading("Item");
            section.AddHeading("Value1");

            section.AddReportingItem("my item1", "value1");
            section.AddReportingItem("my item2", "value2");
            section.AddReportingItem("my item3", "value3");
            section.AddReportingItem("my item4", "value4");
            report.AddSection(section);

            TextSection textsection = new TextSection();
            textsection.SectionTitle = "Third section";
            textsection.SectionTitleBackgroundColor = System.Drawing.Color.Green;
            textsection.Text = "my preformatted text\r\non a new line as well";
            report.AddSection(textsection);

            string html = report.GenerateHtmlReport();
        }
    }
}
