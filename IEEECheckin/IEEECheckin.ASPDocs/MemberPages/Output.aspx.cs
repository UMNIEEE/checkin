using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using IEEECheckin.ASPDocs.Models;
using System.Web.Services;


namespace IEEECheckin.ASPDocs.MemberPages
{
    public partial class Output : System.Web.UI.Page
    {

        public string SubmitDataStr { get { return SubmitData.Text; } set { SubmitData.Text = value; } }
        public string MeetingNameStr { get { return MeetingName.Text; } set { MeetingName.Text = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void SubmitGoogle(object sender, EventArgs e)
        {
            Server.Transfer("~/MemberPages/GoogleDocSelect.aspx");
        }

        protected void SubmitCsv(object sender, EventArgs e)
        {
            try
            {
                string meetingName = "meeting_sign_ins";
                if (!String.IsNullOrWhiteSpace(MeetingNameStr))
                {
                    meetingName = MeetingNameStr;
                    meetingName = meetingName.Replace(' ', '_');
                }
                meetingName += ".csv";

                if (String.IsNullOrWhiteSpace(SubmitDataStr))
                {
                    return; // error
                }

                // clear http header
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();

                // create file content
                XmlDocument xml = JsonConvert.DeserializeXmlNode(SubmitDataStr, "data", false);

                StringBuilder csvStr = new StringBuilder();

                csvStr.AppendLine("Student Id,First Name,Last Name,Email,Date,Meeting");

                foreach (XmlNode node in xml.DocumentElement.ChildNodes)
                {
                    foreach (XmlNode elem in node.ChildNodes)
                    {
                        csvStr.Append(elem.FirstChild.Value + ",");
                    }
                    csvStr.AppendLine();
                }

                // add header to response
                Response.AddHeader("Content-Disposition", "attachment; filename=" + meetingName);
                Response.AddHeader("Content-Length", csvStr.Length.ToString());
                Response.ContentType = "text/plain";

                // write output to response
                Response.Flush();
                Response.Output.Write(csvStr.ToString());
                Response.End();
            }
            catch(Exception ex)
            {

            }
        }

        protected void SubmitJson(object sender, EventArgs e)
        {
            try
            {
                string meetingName = "meeting_sign_ins";
                if (!String.IsNullOrWhiteSpace(MeetingNameStr))
                {
                    meetingName = MeetingNameStr;
                    meetingName = meetingName.Replace(' ', '_');
                }
                meetingName += ".js";

                if (String.IsNullOrWhiteSpace(SubmitDataStr))
                {
                    return; // error
                }

                // clear http header
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();

                // add header to response
                Response.AddHeader("Content-Disposition", "attachment; filename=" + meetingName);
                Response.AddHeader("Content-Length", (SubmitDataStr).Length.ToString());
                Response.ContentType = "text/plain";

                // write output to response
                Response.Flush();
                Response.Output.Write(SubmitDataStr);
                Response.End();
            }
            catch(Exception ex)
            {

            }
        }

        private void GoogleAddRow(SpreadsheetsService service)
        {
            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            SpreadsheetQuery query = new SpreadsheetQuery();

            // Make a request to the API and get all spreadsheets.
            SpreadsheetFeed feed = service.Query(query);

            if (feed.Entries.Count == 0)
            {
                // TODO: There were no spreadsheets, act accordingly.
            }

            // TODO: Choose a spreadsheet more intelligently based on your
            // app's needs.
            SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
            Console.WriteLine(spreadsheet.Title.Text);

            // Get the first worksheet of the first spreadsheet.
            // TODO: Choose a worksheet more intelligently based on your
            // app's needs.
            WorksheetFeed wsFeed = spreadsheet.Worksheets;
            WorksheetEntry worksheet = (WorksheetEntry)wsFeed.Entries[0];

            // Define the URL to request the list feed of the worksheet.
            AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            // Fetch the list feed of the worksheet.
            ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
            ListFeed listFeed = service.Query(listQuery);

            // Create a local representation of the new row.
            ListEntry row = new ListEntry();
            row.Elements.Add(new ListEntry.Custom() { LocalName = "firstname", Value = "Joe" });
            row.Elements.Add(new ListEntry.Custom() { LocalName = "lastname", Value = "Smith" });
            row.Elements.Add(new ListEntry.Custom() { LocalName = "age", Value = "26" });
            row.Elements.Add(new ListEntry.Custom() { LocalName = "height", Value = "176" });

            // Send the new row to the API for insertion.
            service.Insert(listFeed, row);
        }
    }
}