<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="IEEECheckin.ASP._Default" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <section id="login">
        <asp:LoginView runat="server" ViewStateMode="Disabled">
            <AnonymousTemplate>
                <ul>
                    <li><a id="registerLink" runat="server" href="~/Account/Register">Register</a></li>
                    <li><a id="loginLink" runat="server" href="~/Account/Login">Log in</a></li>
                </ul>
            </AnonymousTemplate>
            <LoggedInTemplate>
                <p>
                    Hello, <a runat="server" class="username" href="~/Account/Manage" title="Manage your account">
                        <asp:LoginName runat="server" CssClass="username" /></a>!
                    <asp:LoginStatus runat="server" LogoutAction="Redirect" LogoutText="Log off" LogoutPageUrl="~/" />
                </p>
            </LoggedInTemplate>
        </asp:LoginView>
    </section>
</asp:Content>
