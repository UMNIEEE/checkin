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
    public partial class Checkin : Page
    {
        private string _myConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString;
        private const string _dateFormat = "yyyy-MM-dd";

        private const string _insertPerson = "INSERT INTO ent_person (PersonId, FirstName, LastName, UStudentId, EmailOne, DateCreated, DateLastModified) VALUES " +
            "(@PersonId, @FirstName, @LastName, @UStudentId, @EmailOne, @DateCreated, @DateLastModified);";

        private const string _insertPart = "INSERT INTO eventparticipants (EventParticipantId, EventId, PersonId, DateCreated, DateLastModified) VALUES " +
            "(@EventParticipantId, @EventId, @PersonId, @DateCreated, @DateLastModified);";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                cardTxt.Text = String.Empty;
                firstName.Text = String.Empty;
                lastName.Text = String.Empty;
                studentId.Value = String.Empty;
                email.Text = String.Empty;
            }
        }

        protected void Submit(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                MySqlConnection sqlConn = new MySqlConnection(_myConnectionString);
                sqlConn.Open();

                int personId = -1;
                string eventIdStr = Request["meeting"];
                int eventId = -1;
                if (String.IsNullOrWhiteSpace(eventIdStr) || !Int32.TryParse(eventIdStr, out eventId) || eventId < 0)
                {
                    return; //error
                }
                string dateStr = DateTime.Now.ToString(_dateFormat);

                if (String.IsNullOrWhiteSpace(firstName.Text) || String.IsNullOrWhiteSpace(lastName.Text))
                {
                    return; // error
                }

                // check for studentId. If none given then assume new person
                if (!String.IsNullOrWhiteSpace(studentId.Value))
                {
                    personId = getPersonId(sqlConn, studentId.Value.Trim());
                }


                if (personId == -1) // create new person
                {
                    personId = getMaxId(sqlConn, "personId", "ent_person");
                    if (personId < 0)
                    {
                        return; //error
                    }
                    personId += 1;

                    MySqlCommand cmdPerson = new MySqlCommand();
                    cmdPerson.Connection = sqlConn;
                    cmdPerson.CommandType = System.Data.CommandType.Text;
                    cmdPerson.CommandText = _insertPerson;

                    cmdPerson.Parameters.AddWithValue("@PersonId", personId);
                    cmdPerson.Parameters.AddWithValue("@FirstName", firstName.Text.Trim());
                    cmdPerson.Parameters.AddWithValue("@LastName", lastName.Text.Trim());
                    if (!String.IsNullOrWhiteSpace(studentId.Value))
                        cmdPerson.Parameters.AddWithValue("@UStudentId", studentId.Value.Trim());
                    else
                        cmdPerson.Parameters.AddWithValue("@UStudentId", null);
                    if (!String.IsNullOrWhiteSpace(email.Text))
                        cmdPerson.Parameters.AddWithValue("@EmailOne", email.Text.Trim());
                    else
                        cmdPerson.Parameters.AddWithValue("@EmailOne", null);
                    cmdPerson.Parameters.AddWithValue("@DateCreated", dateStr);
                    cmdPerson.Parameters.AddWithValue("@DateLastModified", dateStr);

                    cmdPerson.ExecuteNonQuery();
                }
                else if (personId < -1)
                {
                    return; //error
                }

                int eventPartId = getMaxId(sqlConn, "EventParticipantId", "eventparticipants");
                if (eventPartId < 0)
                {
                    return; // error
                }
                eventPartId += 1;

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = sqlConn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = _insertPart;

                cmd.Parameters.AddWithValue("@EventParticipantId", eventPartId);
                cmd.Parameters.AddWithValue("@EventId", eventId);
                cmd.Parameters.AddWithValue("@PersonId", personId);
                cmd.Parameters.AddWithValue("@DateCreated", dateStr);
                cmd.Parameters.AddWithValue("@DateLastModified", dateStr);

                cmd.ExecuteNonQuery();

                sqlConn.Close();

                cardTxt.Text = String.Empty;
                firstName.Text = String.Empty;
                lastName.Text = String.Empty;
                studentId.Value = String.Empty;
                email.Text = String.Empty;

                Response.Redirect(@"~/MemberPages/Confirm");
            }
            catch(Exception ex)
            {
                
            }
        }

        private int getPersonId(MySqlConnection sqlConn, string studentId)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = sqlConn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = String.Format("SELECT PersonId FROM ent_person WHERE UStudentId = '{0}' LIMIT 1;", studentId);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows && rdr.Read() && !rdr.IsDBNull(0))
                    {
                        return rdr.GetInt32(0);
                    }
                    else
                        return -1;
                }
            }
            catch(Exception ex)
            {
                return -2;
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
            catch(Exception ex)
            {
                return -1;
            }
        }
    }
}