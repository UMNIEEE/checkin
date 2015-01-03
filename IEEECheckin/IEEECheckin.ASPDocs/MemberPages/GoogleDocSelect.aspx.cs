using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Configuration;
using System.Xml;
using Newtonsoft.Json;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using IEEECheckin.ASPDocs.Models;


namespace IEEECheckin.ASPDocs.MemberPages
{
    
    public partial class GoogleDocSelect : System.Web.UI.Page
    {
        public string SubmitDataStr { get { return SubmitData.Text; } set { SubmitData.Text = value; } }
        public string MeetingNameStr { get { return MeetingName.Text; } set { MeetingName.Text = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                try
                {

                    if(PreviousPage != null)
                    {
                        //SubmitDataStr = PreviousPage.SubmitDataStr;
                        //MeetingNameStr = PreviousPage.MeetingNameStr;
                    }

                    // Make the Auth request to Google
                    SpreadsheetsService service = GoogleOAuth2.GoogleAuthService(Page.Request);
                    if (service == null)
                    {
                        Response.Redirect("~/Account/Login");
                    }

                    // Get list of sheets
                    GoogleListSheets(service);
                    
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void GoogleListSheets(SpreadsheetsService service)
        {
            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            SpreadsheetQuery query = new SpreadsheetQuery();

            // Make a request to the API and get all spreadsheets.
            SpreadsheetFeed feed = service.Query(query);

            List<GoogleSheet> sheetList = new List<GoogleSheet>(feed.Entries.Count);
            sheetList.Add(new GoogleSheet("<Select a Spreadsheet>", (Uri)null));

            // Iterate through all of the spreadsheets returned
            foreach (SpreadsheetEntry entry in feed.Entries)
            {
                // Print the title of this spreadsheet to the screen
                sheetList.Add(new GoogleSheet(entry.Title.Text, new Uri(entry.Id.Uri.Content)));
            }

            SheetList.DataTextField = "Title";
            SheetList.DataValueField = "Uri";
            SheetList.DataSource = sheetList;
            SheetList.DataBind();
        }

        [WebMethod]
        public static string SubmitGoogle(string submitData, string meetingName, string newDocumentName, string selectedUri, string accessToken, string refreshToken)
        {
            // Make the Auth request to Google
            SpreadsheetsService service = GoogleOAuth2.GoogleAuthService(accessToken, refreshToken);
            if (service == null)
            {
                //Response.Redirect("~/Account/Login");
                return "Error - re-login";
            }

            if (String.IsNullOrWhiteSpace(newDocumentName) && String.IsNullOrWhiteSpace(selectedUri))
            {
                return "Error - no document selected"; //error
            }

            if (String.IsNullOrWhiteSpace(submitData))
            {
                return "Error - no data provided"; // error
            }

            // create file content
            XmlDocument xml = JsonConvert.DeserializeXmlNode(submitData, "data", false);

            List<CheckinEntry> entries = new List<CheckinEntry>();

            //"Student Id,First Name,Last Name,Email,Date,Meeting"

            foreach (XmlNode node in xml.DocumentElement.ChildNodes)
            {
                entries.Add(new CheckinEntry()
                {
                    StudentId = node.ChildNodes[0].FirstChild.Value,
                    FirstName = node.ChildNodes[1].FirstChild.Value,
                    LastName = node.ChildNodes[2].FirstChild.Value,
                    Email = node.ChildNodes[3].FirstChild.Value,
                    DateStr = node.ChildNodes[4].FirstChild.Value,
                    Meeting = node.ChildNodes[5].FirstChild.Value
                });
            }

            if(String.IsNullOrWhiteSpace(newDocumentName)) // use pre-existing
            {
                SpreadsheetQuery query = new SpreadsheetQuery(selectedUri);
                // Make a request to the API and get all spreadsheets.
                SpreadsheetFeed feed = service.Query(query);

                if (feed.Entries.Count == 0)
                {
                    return "Error - no spreadsheet found"; // error
                }

                SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];

                // Create new worksheet in selected spreadhseet

                string meeting = "my_meeting_" + DateTime.Now.ToString("yyyy-MM-dd");
                if (!String.IsNullOrWhiteSpace(meetingName))
                    meeting = meetingName;

                meeting = meeting.Replace(" ", "_");

                WorksheetEntry worksheet = new WorksheetEntry();
                worksheet.Title.Text = meeting;
                worksheet.Cols = 6;
                worksheet.Rows = (uint)entries.Count;

                // Send the local representation of the worksheet to the API for
                // creation.  The URL to use here is the worksheet feed URL of our
                // spreadsheet.
                WorksheetFeed wsFeed = spreadsheet.Worksheets;
                service.Insert(wsFeed, worksheet);

                // Create header row
                // Fetch the cell feed of the worksheet.
                CellQuery cellQuery = new CellQuery(worksheet.CellFeedLink);
                cellQuery.MinimumRow = 0;
                cellQuery.MinimumColumn = 0;
                cellQuery.MaximumColumn = 5;
                CellFeed cellFeed = service.Query(cellQuery);

                string[] headers = {"studentid", "firstname", "lastname", "email", "date", "meeting"};

                int i = 0;
                foreach(CellEntry cell in cellFeed.Entries)
                {
                    if (i < headers.Length)
                    {
                        cell.InputValue = headers[i];
                        cell.Update();
                        i++;
                    }
                    else
                        break;
                }


                // Define the URL to request the list feed of the worksheet.
                AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

                // Fetch the list feed of the worksheet.
                ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
                ListFeed listFeed = service.Query(listQuery);

                foreach (CheckinEntry entry in entries)
                {
                    // Create a local representation of the new row.
                    ListEntry row = new ListEntry();
                    row.Elements.Add(new ListEntry.Custom() { LocalName = "studentid", Value = entry.StudentId });
                    row.Elements.Add(new ListEntry.Custom() { LocalName = "firstname", Value = entry.FirstName });
                    row.Elements.Add(new ListEntry.Custom() { LocalName = "lastname", Value = entry.LastName });
                    row.Elements.Add(new ListEntry.Custom() { LocalName = "email", Value = entry.Email });
                    row.Elements.Add(new ListEntry.Custom() { LocalName = "date", Value = entry.DateStr });
                    row.Elements.Add(new ListEntry.Custom() { LocalName = "meeting", Value = entry.Meeting });

                    // Send the new row to the API for insertion.
                    service.Insert(listFeed, row);
                }
                
            }
            else // create new
            {


            }

            return "Success";
        }
    }
}