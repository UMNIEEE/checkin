using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Newtonsoft.Json;
using System.Text;
using System.IO;

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
    }
}