using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.IO;
using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IEEECheckin.ASPDocs.Models;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using Newtonsoft.Json;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Web;
using System.Threading;
using System.Collections.Generic;
using System.Xml.Serialization;



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
        [XmlAttribute]
        public string Title { get; set; }
        [XmlAttribute]
        public string Uri { get; set; }
        [XmlAttribute]
        public string FeedUri { get; set; }
        [XmlAttribute]
        public string Id { get; set; }
        [XmlIgnoreAttribute]
        public GoogleFolder Parent { get; set; }

        public GoogleSheet() { }

        public GoogleSheet(GoogleFolder parent, string id, string title, string uri, string feedUri)
        {
            Parent = parent;
            Id = id;
            Title = title;
            Uri = uri;
            FeedUri = feedUri;
        }
    }

    public class GoogleFolder
    {
        [XmlAttribute]
        public string Title { get; set; }
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string Uri { get; set; }
        [XmlAttribute]
        public int Level { get; set; }
        private int _childCount = 0;
        [XmlAttribute]
        public int ChildrenCount
        { 
            get
            {
                return (_childCount = (Children != null) ? Children.Count : 0);
            }
            set
            {
                _childCount = (Children != null) ? Children.Count : 0;
            }
        }
        public List<GoogleFolder> Children { get; set; }
        private int _sheetCount = 0;
        [XmlAttribute]
        public int SheetCount
        {
            get
            {
                return (_sheetCount = (Sheets != null) ? Sheets.Count : 0);
            }
            set
            {
                _sheetCount = (Sheets != null) ? Sheets.Count : 0;
            }
        }
        public List<GoogleSheet> Sheets { get; set; }
        [XmlIgnoreAttribute]
        public GoogleFolder Parent { get; set; }

        public GoogleFolder()
        {
            Children = new List<GoogleFolder>();
            Sheets = new List<GoogleSheet>();
        }

        public GoogleFolder(GoogleFolder parent, string id, string title, string uri, int level)
        {
            Children = new List<GoogleFolder>();
            Sheets = new List<GoogleSheet>();
            Parent = parent;
            Id = id;
            Title = title;
            Uri = uri;
            Level = level;
        }

    }

    public class CellAddress
    {
        public uint Row { get; set; }
        public uint Col { get; set; }
        public string IdString { get; set; }

        public CellAddress() { }

        public CellAddress(uint row, uint col)
        {
            Row = row;
            Col = col;
            IdString = string.Format("R{0}C{1}", row, col);
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

    public class GoogleClientSecretsData
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string[] redirect_uris { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }

        public GoogleClientSecretsData() { }
    }

    public class GoogleOAuth2
    {

        /// Google authentication flow
        /// 1. Call 'GoogleAuthenticate' from any page. This will return a URI which you should redirect to.
        /// 2. User will be redirected to Google permissions page where they can accept or decline.
        /// 3. User accepts and will be redirected to the 'GoogleRedirectUri' page found in Web.config. This is also set in the Google developer console.
        /// 4. The redirected page will be given an access code as an URL query parameter.
        /// 5. Call 'GoogleAuthToken' from the redirected page to get the access and refresh token.
        /// 6. These tokens are encrypted and saved to the user's cookies for later access and refresh use.
        /// 7. Once authenticated, 'GoogleAuthDrive' and 'GoogleAuthSheets' can be called to get the Drive and Sheets services respectively.
        /// 8. Access tokens last about an hour, of which time a refresh token is needed to be used. If a call in step 7 returns null, call 'GoogleAuthRefresh' to get a new access token.


        /// <summary>
        /// Gets the JSON format for the OAuth2 authentication parameters.
        /// </summary>
        /// <returns>The authentication parameters as a JSON string.</returns>
        public static string GetParametersJson()
        {
            GoogleClientSecretsData secrets = new GoogleClientSecretsData();
            
            secrets.client_id = ConfigurationManager.AppSettings["GoogleClientId"];
            secrets.client_secret = ConfigurationManager.AppSettings["GoogleClientSecret"];
            secrets.redirect_uris = ConfigurationManager.AppSettings["GoogleRedirectUri"].Split(new char[1] { ' ' });
            secrets.auth_uri = "https://accounts.google.com/o/oauth2/auth";
            secrets.token_uri = "https://accounts.google.com/o/oauth2/token";

            string output = JsonConvert.SerializeObject(secrets);
            return "{\"web\": " + output + " }";

        }

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
            parameters.Scope = ConfigurationManager.AppSettings["GoogleDriveScope"];
            parameters.RedirectUri = ConfigurationManager.AppSettings["GoogleRedirectUri"];
            parameters.AccessType = "offline";

            if (forceRefresh)
            {
                parameters.ApprovalPrompt = "force";
            }

            return parameters;
        }

        /// <summary>
        /// Gets the OAuth2 parameters from the provided access and refresh Tokens.
        /// </summary>
        /// <param name="accessToken">The previously obtained and encrypted Google access token.</param>
        /// <param name="refreshToken">The previously obtained and encrypted Google refresh token.</param>
        /// <param name="forceRefresh">If a force refresh should occur.</param>
        /// <returns>The OAuth2 parameters.</returns>
        public static OAuth2Parameters GetParameters(string accessToken, string refreshToken, bool forceRefresh = false)
        {
            // OAuth2Parameters holds all the parameters related to OAuth 2.0.
            OAuth2Parameters parameters = new OAuth2Parameters();

            parameters.ClientId = ConfigurationManager.AppSettings["GoogleClientId"];
            parameters.ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"];
            parameters.Scope = ConfigurationManager.AppSettings["GoogleDriveScope"];
            parameters.RedirectUri = ConfigurationManager.AppSettings["GoogleRedirectUri"];
            parameters.AccessType = "offline";

            if (forceRefresh)
            {
                parameters.ApprovalPrompt = "force";
            }

            FormsAuthenticationTicket tokenTicket = FormsAuthentication.Decrypt(accessToken);
            parameters.AccessToken = tokenTicket.UserData;

            FormsAuthenticationTicket refreshTicket = FormsAuthentication.Decrypt(refreshToken);
            parameters.RefreshToken = refreshTicket.UserData;

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
            string tokenCookieName = ConfigurationManager.AppSettings["GoogleTokenCookie"];
            string refreshCookieName = ConfigurationManager.AppSettings["GoogleRefreshCookie"];

            parameters.ClientId = ConfigurationManager.AppSettings["GoogleClientId"];
            parameters.ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"];
            parameters.Scope = ConfigurationManager.AppSettings["GoogleDriveScope"];
            parameters.RedirectUri = ConfigurationManager.AppSettings["GoogleRedirectUri"];
            parameters.AccessType = "offline";

            if (forceRefresh)
            {
                parameters.ApprovalPrompt = "force";
            }


            if (Request.Cookies[codeCookieName] != null && !String.IsNullOrWhiteSpace(Request.Cookies[codeCookieName].Value))
            {
                FormsAuthenticationTicket codeTicket = FormsAuthentication.Decrypt(Request.Cookies[codeCookieName].Value);
                parameters.AccessCode = codeTicket.UserData;
            }

            if (Request.Cookies[tokenCookieName] != null && !String.IsNullOrWhiteSpace(Request.Cookies[tokenCookieName].Value))
            {
                FormsAuthenticationTicket tokenTicket = FormsAuthentication.Decrypt(Request.Cookies[tokenCookieName].Value);
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
        public static SpreadsheetsService GoogleAuthSheets(HttpRequest Request)
        {
            // Get OAuth parameters (from config and cookies)
            OAuth2Parameters parameters = GoogleOAuth2.GetParameters(Request);
            if (parameters == null || String.IsNullOrWhiteSpace(parameters.AccessToken) || String.IsNullOrWhiteSpace(parameters.RefreshToken))
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
        /// Gets the Spreadsheet Service using the previously obtained authentication tokens.
        /// </summary>
        /// <param name="accessToken">The previously obtained and encrypted Google access token.</param>
        /// <param name="refreshToken">The previously obtained and encrypted Google refresh token.</param>
        /// <returns>The Spreadsheet Service.</returns>
        public static SpreadsheetsService GoogleAuthSheets(string accessToken, string refreshToken)
        {
            // Get OAuth parameters (from config and cookies)
            OAuth2Parameters parameters = GoogleOAuth2.GetParameters(accessToken, refreshToken);
            if (parameters == null || String.IsNullOrWhiteSpace(parameters.AccessToken) || String.IsNullOrWhiteSpace(parameters.RefreshToken))
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
        /// Gets the Drive Service using previously obtained authentication tokens.
        /// </summary>
        /// <param name="Request">The page request.</param>
        /// <returns>The Drive Service.</returns>
        public static DriveService GoogleAuthDrive(HttpRequest Request)
        {
            // Get OAuth parameters (from config and cookies)
            OAuth2Parameters parameters = GoogleOAuth2.GetParameters(Request);
            if (parameters == null || String.IsNullOrWhiteSpace(parameters.AccessToken) || String.IsNullOrWhiteSpace(parameters.RefreshToken))
            {
                return null;
            }

            GoogleAuthorizationCodeFlow flow;
            string secretStr = GetParametersJson();
            using (Stream stream = GenerateStreamFromString(secretStr))
            {
                flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    DataStore = new FileDataStore("Drive.Sample.Store"),
                    ClientSecretsStream = stream,
                    Scopes = ConfigurationManager.AppSettings["GoogleDriveScope"].Split(new char[1] { ' ' })
                });
            }

            TokenResponse tok = new TokenResponse()
            {
                AccessToken = parameters.AccessToken,
                RefreshToken = parameters.RefreshToken,
                Scope = parameters.Scope,
                TokenType = "Bearer",
                ExpiresInSeconds = 3600
            };

            UserCredential cred = new UserCredential(flow, "user-id", tok);

            return new DriveService(new BaseClientService.Initializer
            {
                ApplicationName = ConfigurationManager.AppSettings["ApplicationName"],
                HttpClientInitializer = cred
            });
        }

        /// <summary>
        /// Gets the Drive Service using previously obtained authentication tokens.
        /// </summary>
        /// <param name="accessToken">The previously obtained and encrypted Google access token.</param>
        /// <param name="refreshToken">The previously obtained and encrypted Google refresh token.</param>
        /// <returns>The Drive Service.</returns>
        public static DriveService GoogleAuthDrive(string accessToken, string refreshToken)
        {
            // Get OAuth parameters (from config and cookies)
            OAuth2Parameters parameters = GoogleOAuth2.GetParameters(accessToken, refreshToken);
            if (parameters == null || String.IsNullOrWhiteSpace(parameters.AccessToken) || String.IsNullOrWhiteSpace(parameters.RefreshToken))
            {
                return null;
            }

            GoogleAuthorizationCodeFlow flow;
            string secretStr = GetParametersJson();
            using (Stream stream = GenerateStreamFromString(secretStr))
            {
                flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    DataStore = new FileDataStore("Drive.Sample.Store"),
                    ClientSecretsStream = stream,
                    Scopes = ConfigurationManager.AppSettings["GoogleDriveScope"].Split(new char[1] { ' ' })
                });
            }

            TokenResponse tok = new TokenResponse()
            {
                AccessToken = parameters.AccessToken,
                RefreshToken = parameters.RefreshToken,
                Scope = parameters.Scope,
                TokenType = "Bearer",
                ExpiresInSeconds = 3600
            };

            UserCredential cred = new UserCredential(flow, "user-id", tok);

            return new DriveService(new BaseClientService.Initializer
            {
                ApplicationName = ConfigurationManager.AppSettings["ApplicationName"],
                HttpClientInitializer = cred
            });
        }

        /// <summary>
        /// Generates a stream from a string.
        /// </summary>
        /// <param name="s">The string to generate a stream from.</param>
        /// <returns>A stream consisting of the contents of the provided string.</returns>
        private static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
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
            if(parameters == null)
            {
                return false;
            }

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
            //Response.Cookies[codeCookieName].HttpOnly = true;
            if (!persistent)
                Response.Cookies[codeCookieName].Expires = DateTime.Now.AddHours(expireHours);

            // Once the user authorizes with Google, the request token (from url) can be exchanged
            // for a long-lived access token.
            OAuthUtil.GetAccessToken(parameters);

            // Save access token to secure cookie for later use
            FormsAuthenticationTicket tokenTicket = new FormsAuthenticationTicket(1, tokenCookieName, DateTime.Now, DateTime.Now.AddHours(expireHours), persistent, parameters.AccessToken, FormsAuthentication.FormsCookiePath);
            string encTokenTicket = FormsAuthentication.Encrypt(tokenTicket);
            Response.Cookies.Add(new HttpCookie(tokenCookieName, encTokenTicket));
            //Response.Cookies[tokenCookieName].HttpOnly = true;
            if (!persistent)
                Response.Cookies[tokenCookieName].Expires = DateTime.Now.AddHours(expireHours);

            // Save refresh token to secure cookie for later use
            if (!String.IsNullOrWhiteSpace(parameters.RefreshToken))
            {
                FormsAuthenticationTicket refreshTicket = new FormsAuthenticationTicket(1, refreshCookieName, DateTime.Now, DateTime.Now.AddHours(expireHours), true, parameters.RefreshToken, FormsAuthentication.FormsCookiePath);
                string encRefreshTicket = FormsAuthentication.Encrypt(refreshTicket);
                Response.Cookies.Add(new HttpCookie(refreshCookieName, encRefreshTicket));
                //Response.Cookies[refreshCookieName].HttpOnly = true;
                Response.Cookies[refreshCookieName].Expires = DateTime.Now.AddHours(expireHours);
            }

            return true;
        }

        /// <summary>
        /// Gets a new access token using the previously acquired refresh token.
        /// </summary>
        /// <param name="Request">The page request.</param>
        /// <param name="Response">The page response.</param>
        /// <param name="expireHours">The hours to save the cookie for (default is a year).</param>
        /// <returns>If the refresh was successful.</returns>
        public static bool GoogleAuthRefresh(HttpRequest Request, HttpResponse Response, int expireHours = 8765)
        {
            bool persistent = true;
            if (expireHours > 0)
                persistent = false;

            string tokenCookieName = ConfigurationManager.AppSettings["GoogleTokenCookie"];
            string refreshCookieName = ConfigurationManager.AppSettings["GoogleRefreshCookie"];

            OAuth2Parameters parameters = GoogleOAuth2.GetParameters(Request);
            if (parameters == null || String.IsNullOrWhiteSpace(parameters.RefreshToken))
            {
                return false;
            }

            // Once the user authorizes with Google, the request token (from url) can be exchanged
            // for a long-lived access token.
            OAuthUtil.GetAccessToken(parameters);

            // Save access token to secure cookie for later use
            FormsAuthenticationTicket tokenTicket = new FormsAuthenticationTicket(1, tokenCookieName, DateTime.Now, DateTime.Now.AddHours(expireHours), persistent, parameters.AccessToken, FormsAuthentication.FormsCookiePath);
            string encTokenTicket = FormsAuthentication.Encrypt(tokenTicket);
            Response.Cookies.Add(new HttpCookie(tokenCookieName, encTokenTicket));
            //Response.Cookies[tokenCookieName].HttpOnly = true;
            if (!persistent)
                Response.Cookies[tokenCookieName].Expires = DateTime.Now.AddHours(expireHours);

            // Save refresh token to secure cookie for later use
            if (!String.IsNullOrWhiteSpace(parameters.RefreshToken))
            {
                FormsAuthenticationTicket refreshTicket = new FormsAuthenticationTicket(1, refreshCookieName, DateTime.Now, DateTime.Now.AddHours(expireHours), true, parameters.RefreshToken, FormsAuthentication.FormsCookiePath);
                string encRefreshTicket = FormsAuthentication.Encrypt(refreshTicket);
                Response.Cookies.Add(new HttpCookie(refreshCookieName, encRefreshTicket));
                //Response.Cookies[refreshCookieName].HttpOnly = true;
                Response.Cookies[refreshCookieName].Expires = DateTime.Now.AddHours(expireHours);
            }

            return true;
        }

        /// <summary>
        /// Gets if cookies exist for Google authentication.
        /// </summary>
        /// <param name="Request">The page request.</param>
        /// <returns>If cookies exist for Google authentication.</returns>
        public static bool IsGoogleAuthenticated(HttpRequest Request)
        {
            string tokenCookieName = ConfigurationManager.AppSettings["GoogleTokenCookie"];
            string refreshCookieName = ConfigurationManager.AppSettings["GoogleRefreshCookie"];

            if (Request.Cookies[tokenCookieName] != null && !String.IsNullOrWhiteSpace(Request.Cookies[tokenCookieName].Value)
                && Request.Cookies[refreshCookieName] != null && !String.IsNullOrWhiteSpace(Request.Cookies[refreshCookieName].Value))
                return true;

            return false;
        }

        /// <summary>
        /// Deletes the Google authentication cookies, requiring the user to re-enter their credentials.
        /// </summary>
        /// <param name="Request">The page request.</param>
        public static void GoogleLogOff(HttpRequest Request, HttpResponse Response)
        {
            string codeCookieName = ConfigurationManager.AppSettings["GoogleCodeCookie"];
            string tokenCookieName = ConfigurationManager.AppSettings["GoogleTokenCookie"];
            string refreshCookieName = ConfigurationManager.AppSettings["GoogleRefreshCookie"];

            if (Request.Cookies[codeCookieName] != null)
            {
                HttpCookie aCookie = new HttpCookie(codeCookieName);
                aCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(aCookie);
            }

            if (Request.Cookies[tokenCookieName] != null)
            {
                HttpCookie aCookie = new HttpCookie(tokenCookieName);
                aCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(aCookie);
            }

            if (Request.Cookies[refreshCookieName] != null)
            {
                HttpCookie aCookie = new HttpCookie(refreshCookieName);
                aCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(aCookie);
            }
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
