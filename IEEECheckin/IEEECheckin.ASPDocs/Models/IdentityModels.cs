using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IEEECheckin.ASPDocs.Models;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using System.Web.Security;


namespace IEEECheckin.ASPDocs.Models
{
    // You can add User data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ClaimsIdentity GenerateUserIdentity(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = manager.CreateIdentity(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            return Task.FromResult(GenerateUserIdentity(manager));
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class GoogleSheet
    {
        public string Title { get; set; }
        public Uri Uri { get; set; }

        public GoogleSheet() { }

        public GoogleSheet(string title, Uri uri)
        {
            Title = title;
            Uri = uri;
        }

        public GoogleSheet(string title, string uri)
        {
            Title = title;
            Uri = new Uri(uri);
        }
    }

    public class CheckinEntry
    {
        private const string _dateFormat = "yyyy-MM-dd";

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentId { get; set; }
        public string Email { get; set; }
        public string Meeting { get; set; }
        public string DateStr { get; set; }

        public CheckinEntry() { }

        public CheckinEntry(string firstName, string lastName, string meeting, string date, string email = "", string studentId = "")
        {
            FirstName = firstName;
            LastName = lastName;
            Meeting = meeting;
            DateStr = date;
            Email = email;
            StudentId = studentId;
        }

        public CheckinEntry(string firstName, string lastName, string meeting, int day, int month, int year, string email = "", string studentId = "")
        {
            FirstName = firstName;
            LastName = lastName;
            Meeting = meeting;
            DateStr = String.Format("{0}-{1}-{2}", year, month.ToString("D2"), day.ToString("D2"));
            Email = email;
            StudentId = studentId;
        }

        public CheckinEntry(string firstName, string lastName, string meeting, string day, string month, string year, string email = "", string studentId = "")
        {
            FirstName = firstName;
            LastName = lastName;
            Meeting = meeting;
            DateStr = String.Format("{0}-{1}-{2}", year, month, day);
            Email = email;
            StudentId = studentId;
        }

        public CheckinEntry(string firstName, string lastName, string meeting, DateTime date, string email = "", string studentId = "")
        {
            FirstName = firstName;
            LastName = lastName;
            Meeting = meeting;
            DateStr = date.ToString(_dateFormat);
            Email = email;
            StudentId = studentId;
        }

    }

    public class GoogleOAuth2
    {

        /// <summary>
        /// Gets the OAuth2 parameters, before any authentication has occurred.
        /// </summary>
        /// <param name="forceRefresh">If a force refresh should occur.</param>
        /// <returns>The OAuth2 parameters.</returns>
        public static OAuth2Parameters GetParameters(bool forceRefresh = false)
        {
            // OAuth2Parameters holds all the parameters related to OAuth 2.0.
            OAuth2Parameters parameters = new OAuth2Parameters();

            parameters.ClientId = ConfigurationManager.AppSettings["GoogleClientId"];
            parameters.ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"];
            parameters.Scope = ConfigurationManager.AppSettings["GoogleScope"];
            parameters.RedirectUri = ConfigurationManager.AppSettings["GoogleRedirectUri"];

            if (forceRefresh)
            {
                parameters.AccessType = "offline";
                parameters.ApprovalPrompt = "force";
            }

            return parameters;
        }

        /// <summary>
        /// Gets the OAuth2 parameters, including the authentication tokens after authentication has occurred.
        /// </summary>
        /// <param name="Request">The page request.</param>
        /// <param name="forceRefresh">If a force refresh should occur.</param>
        /// <returns>The OAuth2 parameters.</returns>
        public static OAuth2Parameters GetParameters(HttpRequest Request, bool forceRefresh = false)
        {
            // OAuth2Parameters holds all the parameters related to OAuth 2.0.
            OAuth2Parameters parameters = new OAuth2Parameters();

            string codeCookieName = ConfigurationManager.AppSettings["GoogleCodeCookie"];
            string tokenCookeName = ConfigurationManager.AppSettings["GoogleTokenCookie"];
            string refreshCookieName = ConfigurationManager.AppSettings["GoogleRefreshCookie"];

            parameters.ClientId = ConfigurationManager.AppSettings["GoogleClientId"];
            parameters.ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"];
            parameters.Scope = ConfigurationManager.AppSettings["GoogleScope"];
            parameters.RedirectUri = ConfigurationManager.AppSettings["GoogleRedirectUri"];

            if (forceRefresh)
            {
                parameters.AccessType = "offline";
                parameters.ApprovalPrompt = "force";
            }


            if (Request.Cookies[codeCookieName] != null && !String.IsNullOrWhiteSpace(Request.Cookies[codeCookieName].Value))
            {
                FormsAuthenticationTicket codeTicket = FormsAuthentication.Decrypt(Request.Cookies[codeCookieName].Value);
                parameters.AccessCode = codeTicket.UserData;
            }

            if (Request.Cookies[tokenCookeName] != null && !String.IsNullOrWhiteSpace(Request.Cookies[tokenCookeName].Value))
            {
                FormsAuthenticationTicket tokenTicket = FormsAuthentication.Decrypt(Request.Cookies[tokenCookeName].Value);
                parameters.AccessToken = tokenTicket.UserData;
            }

            if (Request.Cookies[refreshCookieName] != null && !String.IsNullOrWhiteSpace(Request.Cookies[refreshCookieName].Value))
            {
                FormsAuthenticationTicket refreshTicket = FormsAuthentication.Decrypt(Request.Cookies[refreshCookieName].Value);
                parameters.RefreshToken = refreshTicket.UserData;
            }
            
            return parameters;
        }

        /// <summary>
        /// Gets the Spreadsheet Service using the previously obtained authentication tokens.
        /// </summary>
        /// <param name="Request">The page request.</param>
        /// <returns>The Spreadsheet Service.</returns>
        public static SpreadsheetsService GoogleAuthService(HttpRequest Request)
        {
            // Get OAuth parameters (from config and cookies)
            OAuth2Parameters parameters = GoogleOAuth2.GetParameters(Request);
            if (parameters == null)
            {
                return null;
            }

            // Make an OAuth authorized request to Google
            // Initialize the variables needed to make the request
            GOAuth2RequestFactory requestFactory =
                new GOAuth2RequestFactory(null, ConfigurationManager.AppSettings["ApplicationName"], parameters);
            SpreadsheetsService service = new SpreadsheetsService(ConfigurationManager.AppSettings["ApplicationName"]);
            service.RequestFactory = requestFactory;

            return service;
        }

        /// <summary>
        /// Performs the initial step in Google authentication to get the access code.
        /// </summary>
        /// <param name="Request">The page request.</param>
        /// <returns>The Google response URI.</returns>
        public static string GoogleAuthenticate(HttpRequest Request)
        {
            string refreshCookieName = ConfigurationManager.AppSettings["GoogleRefreshCookie"];

            bool forceRefresh = false;
            if (Request.Cookies[refreshCookieName] == null || String.IsNullOrWhiteSpace(Request.Cookies[refreshCookieName].Value))
                forceRefresh = true; // refresh token is gone, so need to force new creation

            return OAuthUtil.CreateOAuth2AuthorizationUrl(GoogleOAuth2.GetParameters(forceRefresh));
        }

        /// <summary>
        /// Performs the second authentication step after receiving the access code.
        /// </summary>
        /// <param name="Request">The page request.</param>
        /// <param name="Response">The page response.</param>
        /// <param name="forceRefresh">If a forced refresh of access privileges should occur.</param>
        /// <param name="expireHours">The hours to save the cookie for (default is a year).</param>
        /// <returns>If the authentication was successful.</returns>
        public static bool GoogleAuthToken(HttpRequest Request, HttpResponse Response, bool forceRefresh = false, int expireHours = 8765)
        {
            bool persistent = true;
            if (expireHours > 0)
                persistent = false;

            string codeCookieName = ConfigurationManager.AppSettings["GoogleCodeCookie"];
            string tokenCookieName = ConfigurationManager.AppSettings["GoogleTokenCookie"];
            string refreshCookieName = ConfigurationManager.AppSettings["GoogleRefreshCookie"];

            if (Request.Cookies[refreshCookieName] == null || String.IsNullOrWhiteSpace(Request.Cookies[refreshCookieName].Value))
                forceRefresh = true; // refresh token is gone, so need to force new creation


            OAuth2Parameters parameters = GoogleOAuth2.GetParameters(forceRefresh);

            // parse access code from url
            if (String.IsNullOrWhiteSpace(Request.QueryString["code"]))
            {
                return false;
            }
            parameters.AccessCode = Request.QueryString["code"];

            // save access code to secure cookie for later use
            FormsAuthenticationTicket codeTicket = new FormsAuthenticationTicket(1, codeCookieName, DateTime.Now, DateTime.Now.AddHours(expireHours), persistent, parameters.AccessCode, FormsAuthentication.FormsCookiePath);
            string encCodeTicket = FormsAuthentication.Encrypt(codeTicket);
            Response.Cookies.Add(new HttpCookie(codeCookieName, encCodeTicket));
            Response.Cookies[codeCookieName].HttpOnly = true;
            if(!persistent)
                Response.Cookies[codeCookieName].Expires = DateTime.Now.AddHours(expireHours);

            // Once the user authorizes with Google, the request token (from url) can be exchanged
            // for a long-lived access token.
            OAuthUtil.GetAccessToken(parameters);

            // Save access token to secure cookie for later use
            FormsAuthenticationTicket tokenTicket = new FormsAuthenticationTicket(1, tokenCookieName, DateTime.Now, DateTime.Now.AddHours(expireHours), persistent, parameters.AccessToken, FormsAuthentication.FormsCookiePath);
            string encTokenTicket = FormsAuthentication.Encrypt(tokenTicket);
            Response.Cookies.Add(new HttpCookie(tokenCookieName, encTokenTicket));
            Response.Cookies[tokenCookieName].HttpOnly = true;
            if (!persistent)
                Response.Cookies[tokenCookieName].Expires = DateTime.Now.AddHours(expireHours);

            // Save refresh token to secure cookie for later use
            if (!String.IsNullOrWhiteSpace(parameters.RefreshToken))
            {
                FormsAuthenticationTicket refreshTicket = new FormsAuthenticationTicket(1, refreshCookieName, DateTime.Now, DateTime.Now.AddHours(expireHours), true, parameters.RefreshToken, FormsAuthentication.FormsCookiePath);
                string encRefreshTicket = FormsAuthentication.Encrypt(refreshTicket);
                Response.Cookies.Add(new HttpCookie(refreshCookieName, encRefreshTicket));
                Response.Cookies[refreshCookieName].HttpOnly = true;
                Response.Cookies[refreshCookieName].Expires = DateTime.Now.AddHours(expireHours);
            }

            return true;
        }

        /// <summary>
        /// Refreshes access to Google authentication using the saved refresh token.
        /// Steps 1 & 2 must have occurred and a valid refresh token must be available, else need to re-authenticate.
        /// </summary>
        /// <param name="Response">The page response.</param>
        /// <param name="expireHours">The hours to save the cookie for (default is a year).</param>
        /// <returns>If the refresh was successful.</returns>
        public static bool GoogleRefreshAccess(HttpResponse Response, int expireHours = 8765)
        {
            bool persistent = true;
            if (expireHours > 0)
                persistent = false;

            string tokenCookieName = ConfigurationManager.AppSettings["GoogleTokenCookie"];
            string refreshCookieName = ConfigurationManager.AppSettings["GoogleRefreshCookie"];

            if (Response.Cookies[refreshCookieName] == null || String.IsNullOrWhiteSpace(Response.Cookies[refreshCookieName].Value))
                return false; // refresh token is gone, so need to force new creation            

            OAuth2Parameters parameters = GoogleOAuth2.GetParameters();

            OAuthUtil.GetAccessToken(parameters);

            // Save access token to secure cookie for later use
            FormsAuthenticationTicket tokenTicket = new FormsAuthenticationTicket(1, tokenCookieName, DateTime.Now, DateTime.Now.AddHours(expireHours), persistent, parameters.AccessToken, FormsAuthentication.FormsCookiePath);
            string encTokenTicket = FormsAuthentication.Encrypt(tokenTicket);
            Response.Cookies.Add(new HttpCookie(tokenCookieName, encTokenTicket));
            Response.Cookies[tokenCookieName].HttpOnly = true;
            if (!persistent)
                Response.Cookies[tokenCookieName].Expires = DateTime.Now.AddHours(expireHours);

            return true;
        }
    }
}

#region Helpers
namespace IEEECheckin.ASPDocs
{
    public static class IdentityHelper
    {
        // Used for XSRF when linking external logins
        public const string XsrfKey = "XsrfId";

        public const string ProviderNameKey = "providerName";
        public static string GetProviderNameFromRequest(HttpRequest request)
        {
            return request.QueryString[ProviderNameKey];
        }

        public const string CodeKey = "code";
        public static string GetCodeFromRequest(HttpRequest request)
        {
            return request.QueryString[CodeKey];
        }

        public const string UserIdKey = "userId";
        public static string GetUserIdFromRequest(HttpRequest request)
        {
            return System.Web.HttpUtility.UrlDecode(request.QueryString[UserIdKey]);
        }

        public static string GetResetPasswordRedirectUrl(string code, HttpRequest request)
        {
            var absoluteUri = "/Account/ResetPassword?" + CodeKey + "=" + System.Web.HttpUtility.UrlEncode(code);
            return new Uri(request.Url, absoluteUri).AbsoluteUri.ToString();
        }

        public static string GetUserConfirmationRedirectUrl(string code, string userId, HttpRequest request)
        {
            var absoluteUri = "/Account/Confirm?" + CodeKey + "=" + System.Web.HttpUtility.UrlEncode(code) + "&" + UserIdKey + "=" + System.Web.HttpUtility.UrlEncode(userId);
            return new Uri(request.Url, absoluteUri).AbsoluteUri.ToString();
        }

        private static bool IsLocalUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\'))) || (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }

        public static void RedirectToReturnUrl(string returnUrl, HttpResponse response)
        {
            if (!String.IsNullOrEmpty(returnUrl) && IsLocalUrl(returnUrl))
            {
                response.Redirect(returnUrl);
            }
            else
            {
                response.Redirect("~/");
            }
        }
    }
}
#endregion
