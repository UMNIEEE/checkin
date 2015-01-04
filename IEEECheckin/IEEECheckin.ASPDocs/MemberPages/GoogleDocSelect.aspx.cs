using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using Google.Apis.Drive.v2;
using GDrive = Google.Apis.Drive.v2.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using IEEECheckin.ASPDocs.Models;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Drive.v2.Data;
using System.Xml.Serialization;
using System.Text;

namespace IEEECheckin.ASPDocs.MemberPages
{
    
    public partial class GoogleDocSelect : System.Web.UI.Page
    {
        public string SubmitDataStr { get { return SubmitData.Value; } set { SubmitData.Value = value; } }
        public string MeetingNameStr { get { return MeetingName.Value; } set { MeetingName.Value = value; } }
        public string MeetingDateStr { get { return MeetingDate.Value; } set { MeetingDate.Value = value; } }
        public string MeetingNameDate { get { return MeetingNameStr + " " + MeetingDateStr; } }

        public const string _FolderMime = "application/vnd.google-apps.folder";
        public const string _SheetMime = "application/vnd.google-apps.spreadsheet";

        private List<GoogleSheet> _sheetList;

        /// <summary>
        /// Application logic should manage users authentication. This sample works with only one user. You can change
        /// it by retrieving data from the session.
        /// </summary>
        private const string UserId = "user-id";

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
                    SpreadsheetsService sheetsService = GoogleOAuth2.GoogleAuthSheets(Page.Request);
                    if (sheetsService == null)
                    {
                        return;
                    }
                    _sheetList = GoogleRetrieveAllSheets(sheetsService);

                    // Get list of sheets
                    DriveService driveService = GoogleOAuth2.GoogleAuthDrive(Page.Request);
                    if (driveService == null)
                    {
                        return;
                    }
                    //List<GDrive.File> fileList = GoogleRetrieveAllSheets(driveService);
                    GoogleFolder root = new GoogleFolder(null, "root", "root", "", 0);
                    root = GoogleRetrieveSheetTree(driveService, root, 1);

                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(root.GetType());
                    using(var stream = new StreamWriter(@"D:\Bryan Stadick\Downloads\output.xml", false))
                    {
                        x.Serialize(stream, root);
                    }

                    XmlDocument doc = new XmlDocument();
                    string xmlStr = SerializeXml<GoogleFolder>(root);
                    xmlStr = xmlStr.Replace("<Children>", "").Replace("<Children />", "").Replace("</Children>", "");
                    xmlStr = xmlStr.Replace("<Sheets>", "").Replace("<Sheets />", "").Replace("</Sheets>", "");
                    doc.LoadXml(xmlStr);

                    SheetTreeDataSource.Data = xmlStr;
                    SheetTree.DataBind();
                    
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static string SerializeXml<T>(T value)
        {

            if (value == null)
            {
                return null;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        private List<GoogleSheet> GoogleRetrieveAllSheets(SpreadsheetsService service)
        {
            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            SpreadsheetQuery query = new SpreadsheetQuery();

            // Make a request to the API and get all spreadsheets.
            SpreadsheetFeed feed = service.Query(query);

            List<GoogleSheet> sheetList = new List<GoogleSheet>(feed.Entries.Count);
            sheetList.Add(new GoogleSheet(null, "", "<Select a Spreadsheet>", "", ""));

            // Iterate through all of the spreadsheets returned
            foreach (SpreadsheetEntry entry in feed.Entries)
            {
                // Print the title of this spreadsheet to the screen
                sheetList.Add(new GoogleSheet(null, entry.Id.Uri.Content.Split('/').Last(), entry.Title.Text, entry.Id.Uri.Content, entry.Id.Uri.Content));
            }

            return sheetList;
        }

        /// <summary>
        /// Retrieve a list of File resources.
        /// </summary>
        /// <param name="service">Drive API service instance.</param>
        /// <returns>List of File resources.</returns>
        private List<GDrive.File> GoogleRetrieveAllSheets(DriveService service)
        {
            List<GDrive.File> result = new List<GDrive.File>();
            FilesResource.ListRequest request = service.Files.List();
            request.Q = String.Format("mimeType='{0}'", _SheetMime);

            do
            {
                try
                {
                    GDrive.FileList files = request.Execute();

                    result.AddRange(files.Items);
                    request.PageToken = files.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            return result;
        }

        public GoogleFolder GoogleRetrieveSheetTree(DriveService service, GoogleFolder root, int level)
        {
            // Start with relative root folder
            ChildrenResource.ListRequest request = service.Children.List(root.Id);
            // Request immediate folder children of the relative root folder
            request.Q = String.Format("mimeType='{0}'", _FolderMime);

            do {
                try
                {
                    ChildList children = request.Execute();

                    foreach (ChildReference child in children.Items)
                    {
                        // Get child folder metadata
                        GDrive.File file = service.Files.Get(child.Id).Execute();
                        // Add folder to relative root children folder list
                        root.Children.Add(new GoogleFolder(root, child.Id, file.Title, child.ChildLink, level));
                    }

                    request.PageToken = children.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));        

            // Get list of sheets in the relative root folder
            FilesResource.ListRequest requestFile = service.Files.List();
            requestFile.Q = String.Format("mimeType='{0}' and '{1}' in parents", _SheetMime, root.Id);

            do
            {
                try
                {
                    GDrive.FileList files = requestFile.Execute();

                    foreach(GDrive.File file in files.Items)
                    {
                        // Get feed uri for current sheet
                        string feedUri = "";
                        if(_sheetList != null && _sheetList.Count > 0)
                        {
                            feedUri = (from feed in _sheetList where feed.Id.Equals(file.Id) select feed.FeedUri).First();
                        }
                        // Add sheet to relative root sheet list
                        root.Sheets.Add(new GoogleSheet(root, file.Id, file.Title + ".sheet", file.SelfLink, feedUri));
                    }
                    request.PageToken = files.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            // Recurse through the children folders
            for(int i = 0; i < root.Children.Count; i++)
            {
                root.Children[i] = GoogleRetrieveSheetTree(service, root.Children[i], level + 1);
            }

            // Trim away folders that don't contain children and don't contain sheets
            List<GoogleFolder> notNullChildren = new List<GoogleFolder>();
            foreach(GoogleFolder folder in root.Children)
            {
                if (folder != null)
                    notNullChildren.Add(folder);
            }
            root.Children.Clear();
            root.Children.AddRange(notNullChildren);

            // Return null if not sheets and no children with sheets
            if (root.ChildrenCount > 0 || root.SheetCount > 0)
                return root;
            else
                return null;
        }

        protected void SubmitGoogle(object sender, EventArgs e)
        {
            try
            {
                string newDocumentName = NewDocument.Text;
                if(!SheetTree.SelectedNode.Text.Contains(".sheet"))
                {
                    return;
                }
                string selectedUri = SheetTree.SelectedNode.Value;
                string submitData = SubmitDataStr;
                string meetingName = MeetingNameDate;

                // Make the Auth request to Google
                SpreadsheetsService service = GoogleOAuth2.GoogleAuthSheets(Page.Request);
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

                string script = string.Format("alert('Data added successfully.'); window.location.href = 'Output.aspx';");
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
            if (String.IsNullOrWhiteSpace(selectedUri))
                return null;

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