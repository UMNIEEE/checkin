using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace IEEECheckin.ASP
{
    class Event
    {
        private const string _dateFormat = "yyyy-MM-dd";

        public int EventId { get; private set; }
        public string EventName { get; private set; }
        public DateTime EventDate { get; private set; }
        public string EventNameDate { get { return String.Format("{0} {1}", EventName, EventDate.ToString(_dateFormat)); } }

        public Event()
        {
            EventId = -1;
        }

        public Event(int id, string name, DateTime date)
        {
            EventId = id;
            EventName = name;
            EventDate = date;
        }
    }

    public partial class Menu : Page
    {
        private string _myConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString;
        private const string _dateFormat = "yyyy-MM-dd";
        private const string _insertStr = "INSERT INTO ent_event (EventId, Name, Type, StartDate, EndDate, DateCreated, DateLastModified) VALUES " +
            "(@EventId, @Name, 1, @StartDate, @EndDate, @DateCreated, @DateLastModified);";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                List<Event> events = new List<Event>();
                
                MySqlConnection sqlConn = new MySqlConnection(_myConnectionString);
                sqlConn.Open();

                DateTime date = DateTime.Now;
                date = date.AddYears(-1);

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = sqlConn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = String.Format("SELECT EventId, Name, StartDate FROM Ent_Event WHERE StartDate >= '{0}' ORDER BY StartDate DESC;", date.ToString(_dateFormat));
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    string eventName = "";
                    int eventId = -1;
                    DateTime eventDate = DateTime.Now;
                    while(rdr.Read())
                    {
                        eventId = -1;
                        eventName = "";

                        if (!rdr.IsDBNull(0))
                            eventId = rdr.GetInt32("EventId");
                        if (!rdr.IsDBNull(1))
                            eventName = rdr.GetString("Name");
                        if (!rdr.IsDBNull(2))
                            eventDate = rdr.GetDateTime("StartDate");

                        if(eventId >= 0 && !String.IsNullOrWhiteSpace(eventName))
                            events.Add(new Event(eventId, eventName, eventDate));
                    }
                }

                meetingDropdown.DataTextField = "EventNameDate";
                meetingDropdown.DataValueField = "EventId";
                meetingDropdown.DataSource = events;
                meetingDropdown.DataBind();

                sqlConn.Close();
            }
            catch(Exception ex)
            {

            }
        }

        protected void Submit(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                if (String.IsNullOrWhiteSpace(newMeeting.Text)) // no meeting string so use existing meeting
                {
                    int meetingId;
                    if (!Int32.TryParse(meetingDropdown.SelectedValue, out meetingId))
                    {
                        return; // error
                    }
                    string meetingName = meetingDropdown.SelectedItem.Text;
                    Regex rgx = new Regex(@"(\d{4}-\d{2}-\d{2})$");
                    string[] split = rgx.Split(meetingName);
                    if (split != null && split.Length >= 2)
                    {
                        meetingName = split[0];
                        Response.Redirect(String.Format(@"~/MemberPages/Checkin?meeting={0}&name={1}", meetingId, meetingName));
                    }
                    else
                    {
                        return; // error
                    }
                }
                else // create new meeting
                {
                    MySqlConnection sqlConn = new MySqlConnection(_myConnectionString);
                    sqlConn.Open();

                    int meetingId = getMaxId(sqlConn, "EventId", "ent_event");
                    if (meetingId < 0)
                    {
                        return; // error
                    }
                    meetingId += 1;

                    string dateStr = DateTime.Now.ToString(_dateFormat);

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = sqlConn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = _insertStr;

                    cmd.Parameters.AddWithValue("@EventId", meetingId);
                    cmd.Parameters.AddWithValue("@Name", newMeeting.Text.Trim());
                    cmd.Parameters.AddWithValue("@StartDate", dateStr);
                    cmd.Parameters.AddWithValue("@EndDate", dateStr);
                    cmd.Parameters.AddWithValue("@DateCreated", dateStr);
                    cmd.Parameters.AddWithValue("@DateLastModified", dateStr);

                    cmd.ExecuteNonQuery();

                    sqlConn.Close();

                    Response.Redirect(String.Format(@"~/MemberPages/Checkin?meeting={0}&name={1}", meetingId, newMeeting.Text.Trim()));
                }
            }
            catch(Exception ex)
            {

            }
        }

        private int getMaxId(MySqlConnection sqlConn, string column, string table)
        {
            try
            {
                string cmdStr = String.Format("SELECT Max({0}) FROM {1};", column, table);
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = sqlConn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = cmdStr;
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows && rdr.Read() && !rdr.IsDBNull(0))
                    {
                        return rdr.GetInt32(0);
                    }
                    else
                        return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}