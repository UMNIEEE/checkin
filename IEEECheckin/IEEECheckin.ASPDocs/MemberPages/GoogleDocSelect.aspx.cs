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
        public string SubmitDataStr { get { return SubmitData.Value; } set { SubmitData.Value = value; } }
        public string MeetingNameStr { get { return MeetingName.Value; } set { MeetingName.Value = value; } }
        public string MeetingDateStr { get { return MeetingDate.Value; } set { MeetingDate.Value = value; } }
        public string MeetingNameDate { get { return MeetingNameStr + " " + MeetingDateStr; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                try
                {

                    if(PreviousPage != null)
                    {
                        SubmitDataStr = PreviousPage.SubmitDataStr;
                        MeetingNameStr = PreviousPage.MeetingNameStr;
                        MeetingDateStr = PreviousPage.MeetingDateStr;
                    }

                    // Make the Auth request to Google
                    SpreadsheetsService service = GoogleOAuth2.GoogleAuthService(Page.Request);
                    if (service == null)
                    {
                        Response.Redirect("~/Account/Login.aspx");
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

        protected void SubmitGoogle(object sender, EventArgs e)
        {
            try
            {
                string newDocumentName = NewDocument.Text;
                string selectedUri = SheetList.SelectedValue;
                string submitData = SubmitDataStr;
                string meetingName = MeetingNameDate;

                // Make the Auth request to Google
                SpreadsheetsService service = GoogleOAuth2.GoogleAuthService(Page.Request);
                if (service == null)
                {
                    //Response.Redirect("~/Account/Login");
                    //return "Error - re-login";
                    return;
                }

                if (String.IsNullOrWhiteSpace(newDocumentName) && String.IsNullOrWhiteSpace(selectedUri))
                {
                    //return "Error - no document selected"; //error
                    return;
                }

                if (String.IsNullOrWhiteSpace(submitData))
                {
                    //return "Error - no data provided"; // error
                    return;
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

                if (String.IsNullOrWhiteSpace(newDocumentName)) // use pre-existing
                {
                    SpreadsheetEntry spreadsheet = QuerySpreadsheet(service, selectedUri);
                    if (spreadsheet == null)
                    {
                        return; // error
                    }

                    // Create new worksheet in selected spreadhseet

                    string meeting = "my_meeting_" + DateTime.Now.ToString("yyyy-MM-dd");
                    if (!String.IsNullOrWhiteSpace(meetingName))
                        meeting = meetingName;

                    meeting = meeting.Replace(" ", "_");

                    // Find if worksheet already present
                    WorksheetEntry worksheet = QueryWorksheet(service, selectedUri, meeting);

                    // not present so create new worksheet
                    if (worksheet == null)
                    {

                        worksheet = new WorksheetEntry();
                        worksheet.Title.Text = meeting;
                        worksheet.Cols = 6;
                        worksheet.Rows = 2;

                        // Send the local representation of the worksheet to the API for
                        // creation.  The URL to use here is the worksheet feed URL of our
                        // spreadsheet.
                        WorksheetFeed wsFeed = spreadsheet.Worksheets;
                        service.Insert(wsFeed, worksheet);


                        // update worksheet query
                        worksheet = QueryWorksheet(service, selectedUri, meeting);
                        if (worksheet == null)
                        {
                            return; // error
                        }

                        // Create header row
                        // Fetch the cell feed of the worksheet.
                        CellQuery cellQuery = new CellQuery(worksheet.CellFeedLink);
                        CellFeed cellFeed = service.Query(cellQuery);

                        Dictionary<string, string> headers = new Dictionary<string, string>();
                        headers.Add("R1C1", "studentid");
                        headers.Add("R1C2", "firstname");
                        headers.Add("R1C3", "lastname");
                        headers.Add("R1C4", "email");
                        headers.Add("R1C5", "date");
                        headers.Add("R1C6", "meeting");

                        List<CellAddress> cellAddrs = new List<CellAddress>();
                        for (uint col = 1; col <= 6; ++col)
                        {
                            cellAddrs.Add(new CellAddress(1, col));
                        }

                        // Prepare the update
                        // GetCellEntryMap is what makes the update fast.
                        Dictionary<String, CellEntry> cellEntries = GetCellEntryMap(service, cellFeed, cellAddrs);

                        CellFeed batchRequest = new CellFeed(cellQuery.Uri, service);
                        foreach (CellAddress cellAddr in cellAddrs)
                        {
                            CellEntry batchEntry = cellEntries[cellAddr.IdString];
                            batchEntry.InputValue = headers[cellAddr.IdString];
                            batchEntry.BatchData = new GDataBatchEntryData(cellAddr.IdString, GDataBatchOperationType.update);
                            batchRequest.Entries.Add(batchEntry);
                        }

                        // Submit the update
                        CellFeed batchResponse = (CellFeed)service.Batch(batchRequest, new Uri(cellFeed.Batch));

                        // update worksheet query again
                        worksheet = QueryWorksheet(service, selectedUri, meeting);
                        if (worksheet == null)
                        {
                            return; // error
                        }

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

                string script = string.Format("alert('Data added successfully.'); window.location.href = 'Menu.aspx';");
                Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "alert", script, true);
            }
            catch(Exception ex)
            {
                string script = string.Format("alert('Request not logged: {0}');", ex.Message);
                Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "alert", script, true);
            }
        }

        private SpreadsheetEntry QuerySpreadsheet(SpreadsheetsService service, string selectedUri)
        {
            SpreadsheetQuery query = new SpreadsheetQuery(selectedUri);
            // Make a request to the API and get all spreadsheets.
            SpreadsheetFeed feed = service.Query(query);

            if (feed.Entries.Count == 0)
            {
                return null;
            }

            return (SpreadsheetEntry)feed.Entries[0];
        }

        private WorksheetEntry QueryWorksheet(SpreadsheetsService service, string selectedUri, string worksheetName)
        {
            // Re-query for the spreadsheet and updated worksheet feed
            SpreadsheetEntry spreadsheet = QuerySpreadsheet(service, selectedUri);
            if (spreadsheet == null)
            {
                return null; // error
            }

            // Make a request to the API to fetch information about all
            // worksheets in the spreadsheet.
            WorksheetFeed wsFeed = spreadsheet.Worksheets;

            // Iterate through each worksheet in the spreadsheet.
            WorksheetEntry worksheet = null;
            foreach (WorksheetEntry entry in wsFeed.Entries)
            {
                if (entry.Title.Text.Equals(worksheetName))
                {
                    worksheet = entry;
                    break;
                }
            }

            return worksheet;
        }

        private static Dictionary<String, CellEntry> GetCellEntryMap(SpreadsheetsService service, CellFeed cellFeed, List<CellAddress> cellAddrs)
        {
            CellFeed batchRequest = new CellFeed(new Uri(cellFeed.Self), service);
            foreach (CellAddress cellId in cellAddrs)
            {
                CellEntry batchEntry = new CellEntry(cellId.Row, cellId.Col, cellId.IdString);
                batchEntry.Id = new AtomId(string.Format("{0}/{1}", cellFeed.Self, cellId.IdString));
                batchEntry.BatchData = new GDataBatchEntryData(cellId.IdString, GDataBatchOperationType.query);
                batchRequest.Entries.Add(batchEntry);
            }

            CellFeed queryBatchResponse = (CellFeed)service.Batch(batchRequest, new Uri(cellFeed.Batch));

            Dictionary<String, CellEntry> cellEntryMap = new Dictionary<String, CellEntry>();
            foreach (CellEntry entry in queryBatchResponse.Entries)
            {
                cellEntryMap.Add(entry.BatchData.Id, entry);
                Console.WriteLine("batch {0} (CellEntry: id={1} editLink={2} inputValue={3})", entry.BatchData.Id, entry.Id, entry.EditUri, entry.InputValue);
            }

            return cellEntryMap;
        }
    }
}