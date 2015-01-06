using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

//username: ieeegofirst
//password: 0ed2df89a5!

namespace IEEECheckin.ASP.Account
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register";
            OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];

            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            }
        }

        protected void login_LoggedIn(object sender, EventArgs e)
        {
            string req = Request["ReturnUrl"];
            if (login.RememberMeSet)
                FormsAuthentication.SetAuthCookie(login.UserName, true);
            if (String.IsNullOrWhiteSpace(req) || req.Contains("Default"))
                Response.Redirect("~/MemberPages/Menu");
        }
    }
}