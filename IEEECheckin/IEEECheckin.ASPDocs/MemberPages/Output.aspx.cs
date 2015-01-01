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


namespace IEEECheckin.ASPDocs.MemberPages
{
    public partial class Output : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void SubmitCsv(object sender, EventArgs e)
        {
            try
            {
                string meetingName = "meeting_sign_ins";
                if (!String.IsNullOrWhiteSpace(meeting.Value))
                    meetingName = meeting.Value.Replace(' ', '_');
                meetingName += ".csv";

                if (String.IsNullOrWhiteSpace(submitData.Value))
                {
                    return; // error
                }

                // clear http header
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();

                // create file content
                XmlDocument xml = JsonConvert.DeserializeXmlNode(submitData.Value, "data", false);

                StringBuilder csvStr = new StringBuilder();

                csvStr.AppendLine("Student Id,First Name,Last Name,Email,Day,Month,Year,Meeting");

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
                if (!String.IsNullOrWhiteSpace(meeting.Value))
                    meetingName = meeting.Value.Replace(' ', '_');
                meetingName += ".js";

                if (String.IsNullOrWhiteSpace(submitData.Value))
                {
                    return; // error
                }

                // clear http header
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();

                // add header to response
                Response.AddHeader("Content-Disposition", "attachment; filename=" + meetingName);
                Response.AddHeader("Content-Length", submitData.Value.Length.ToString());
                Response.ContentType = "text/plain";

                // write output to response
                Response.Flush();
                Response.Output.Write(submitData.Value);
                Response.End();
            }
            catch(Exception ex)
            {

            }
        }

        protected void SubmitGoogle(object sender, EventArgs e)
        {
            try
            {
                // Get OAuth parameters (from config and cookies)
                OAuth2Parameters parameters = GoogleOAuth2.GetParameters(Page.Request);
                if (parameters == null)
                {
                    return; // error
                }

                // Make an OAuth authorized request to Google
                // Initialize the variables needed to make the request
                GOAuth2RequestFactory requestFactory =
                    new GOAuth2RequestFactory(null, ConfigurationManager.AppSettings["ApplicationName"], parameters);
                SpreadsheetsService service = new SpreadsheetsService(ConfigurationManager.AppSettings["ApplicationName"]);
                service.RequestFactory = requestFactory;

                // Make the request to Google
                GoogleAddRow(service);

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