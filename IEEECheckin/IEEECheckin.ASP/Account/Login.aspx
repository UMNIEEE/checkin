<%@ Page Title="Log in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="IEEECheckin.ASP.Account.Login" %>
<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h1 class="post-header">Log in</h1>
    <p class="section-header">Use a local account to login <i class="fa fa-certificate"></i></p> 
    <div class="boxed-section margin-lg-after">
        <asp:Login  ID="login" runat="server" ViewStateMode="Disabled" RenderOuterTable="false" DestinationPageUrl="~/MemberPages/Menu" OnLoggedIn="login_LoggedIn" PasswordRecoveryText="Forgot Password?" PasswordRecoveryUrl="~/Account/RecoverPassword">
            <LayoutTemplate>
                <p class="validation-summary-errors">
                    <asp:Literal runat="server" ID="FailureText" />
                </p>
                <fieldset>
                    <ol>
                        <li>
                            <asp:Label runat="server" AssociatedControlID="UserName">User name</asp:Label>
                            <asp:TextBox class="form-control input-lg margin-sm-after" runat="server" ID="UserName" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName" CssClass="field-validation-error" ErrorMessage="The user name field is required." />
                        </li>
                        <li>
                            <asp:Label runat="server" AssociatedControlID="Password">Password</asp:Label>
                            <asp:TextBox class="form-control input-lg margin-sm-after" runat="server" ID="Password" TextMode="Password" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="field-validation-error" ErrorMessage="The password field is required." />
                        </li>
                        <li>
                            <table>
                                <tr>
                                    <td><asp:CheckBox class="form-control margin-sm-after" runat="server" ID="RememberMe" /></td>
                                    <td><asp:Label runat="server" AssociatedControlID="RememberMe" CssClass="checkbox">Remember me?</asp:Label></td>
                                </tr>
                            </table>
                        </li>
                    </ol>
                    <asp:Button class="form-control input-lg btn btn-info check-in" runat="server" CommandName="Login" Text="Log in" />
                </fieldset>
            </LayoutTemplate>
        </asp:Login>
        <br />
        <p>
            <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Register</asp:HyperLink>
            if you don't have an account.
        </p>
        <p><asp:HyperLink Text="Forgot Password?" NavigateUrl="~/Account/RecoverPassword" runat="server" /></p>
    </div>
    <p class="section-header">Use another service to log in <i class="fa fa-key"></i></p> 
    <div class="boxed-section margin-lg-after">
        <section id="socialLoginForm">
            <uc:OpenAuthProviders runat="server" ID="OpenAuthLogin" />
        </section>
    </div>
</asp:Content>
