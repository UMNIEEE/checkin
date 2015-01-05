using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Configuration;
using GDrive = Google.Apis.Drive.v2.Data;
using Google.Apis.Drive.v2;
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
        public string FolderTreeXmlStr { get { return FolderTreeXml.Value; } set { FolderTreeXml.Value = value; } }

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
                        FolderTreeXmlStr = PreviousPage.FolderTreeXmlStr;
                    }

                    SheetTreeDataSource.Data = FolderTreeXmlStr;
                    SheetTree.DataBind();
                    
                }
                catch (Exception ex)
                {

                }
            }
        }

        /// <summary>
        /// Submit button handler. Creates file contents and adds to Google Drive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SubmitGoogle(object sender, EventArgs e)
        {
            try
            {
                string newDocumentName = NewDocument.Text;
                string selectedUri = "";
                bool isSheet = false;

                if (SheetTree.SelectedNode == null)
                {
                    selectedUri = "root";
                }
                else
                {
                    selectedUri = SheetTree.SelectedNode.Value;
                    isSheet = SheetTree.SelectedNode.Text.Contains(".sheet");
                }
                
                string submitData = SubmitDataStr;
                string meetingName = MeetingNameDate;

                if (String.IsNullOrWhiteSpace(submitData) ||
                    (String.IsNullOrWhiteSpace(newDocumentName) && String.IsNullOrWhiteSpace(selectedUri)) ||
                    (String.IsNullOrWhiteSpace(newDocumentName) && !isSheet))
                    throw new Exception("Missing data, document name or selected document/folder.");

                // Make the Auth request to Google
                SpreadsheetsService sheetService = GoogleOAuth2.GoogleAuthSheets(Page.Request);
                if (sheetService == null)
                    throw new Exception("Google authentication not acquired. Try logging off and in again.");

                // Make the Auth request to Google
                DriveService driveService = GoogleOAuth2.GoogleAuthDrive(Page.Request);
                if (driveService == null)
                    throw new Exception("Google authentication not acquired. Try logging off and in again.");

                // create file contents
                List<CheckinEntry> entries = GoogleDriveHelpers.CreateFileContent(submitData);
                if(entries == null)
                    throw new Exception("Server exception: code 5");

                // create new spreadsheet if need be
                if (!String.IsNullOrWhiteSpace(newDocumentName) && !isSheet)
                {
                    GDrive.File newFile = GoogleDriveHelpers.CreateFile(driveService, newDocumentName, selectedUri);
                    if(newFile == null)
                        throw new Exception("Server exception: code 6");
                    // get spreadsheet feed
                    List<GoogleSheet> sheetList = GoogleDriveHelpers.GoogleRetrieveAllSheets(sheetService);
                    selectedUri = "";
                    if (sheetList != null && sheetList.Count > 0)
                        selectedUri = (from feed in sheetList where feed.Id.Equals(newFile.Id) select feed.FeedUri).First();
                }

                // create new worksheet in spreadsheet
                WorksheetEntry worksheet = GoogleDriveHelpers.CreateWorksheet(sheetService, selectedUri, meetingName);
                if(worksheet == null)
                    throw new Exception("Server exception: code 7");

                // Add content to worksheet
                if (!GoogleDriveHelpers.AddContentToWorksheet(sheetService, worksheet, entries))
                    throw new Exception("Server exception: code 8");
                
                // output page redirect
                string script = string.Format("alert('Data added successfully.');");
                Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "alert", script, true);
            }
            catch(Exception ex)
            {
                string script = string.Format("alert('Request not logged: {0}');", ex.Message);
                Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "alert", script, true);
            }
        }

        
    }
}