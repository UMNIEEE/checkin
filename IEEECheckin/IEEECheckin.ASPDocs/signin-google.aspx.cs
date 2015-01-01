using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Configuration;
using Newtonsoft.Json;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using IEEECheckin.ASPDocs.Models;

namespace IEEECheckin.ASPDocs
{
    public partial class google_signin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    OAuth2Parameters parameters = GoogleOAuth2.GetParameters();
                    // parse access code from url

                    string codeCookieName = ConfigurationManager.AppSettings["GoogleCodeCookie"];
                    string tokenCookeName = ConfigurationManager.AppSettings["GoogleTokenCookie"];

                    if (String.IsNullOrWhiteSpace(Request.QueryString["code"]))
                    {
                        Response.Redirect("~/MemberPages/Menu");
                    }
                    parameters.AccessCode = Request.QueryString["code"];

                    // save access code to secure cookie for later use
                    FormsAuthenticationTicket codeTicket = new FormsAuthenticationTicket(1, codeCookieName, DateTime.Now, DateTime.Now.AddHours(12), false, parameters.AccessCode, FormsAuthentication.FormsCookiePath);
                    string encCodeTicket = FormsAuthentication.Encrypt(codeTicket);
                    Response.Cookies.Add(new HttpCookie(codeCookieName, encCodeTicket));
                    Response.Cookies[codeCookieName].HttpOnly = true;
                    Response.Cookies[codeCookieName].Expires = DateTime.Now.AddHours(12);

                    // Once the user authorizes with Google, the request token (from url) can be exchanged
                    // for a long-lived access token.
                    OAuthUtil.GetAccessToken(parameters);

                    // save access token to secure cookie for later use
                    FormsAuthenticationTicket tokenTicket = new FormsAuthenticationTicket(1, tokenCookeName, DateTime.Now, DateTime.Now.AddHours(12), false, parameters.AccessToken, FormsAuthentication.FormsCookiePath);
                    string encTokenTicket = FormsAuthentication.Encrypt(tokenTicket);
                    Response.Cookies.Add(new HttpCookie(tokenCookeName, encTokenTicket));
                    Response.Cookies[tokenCookeName].HttpOnly = true;
                    Response.Cookies[tokenCookeName].Expires = DateTime.Now.AddHours(12);

                    Response.Redirect("~/MemberPages/Menu");
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}