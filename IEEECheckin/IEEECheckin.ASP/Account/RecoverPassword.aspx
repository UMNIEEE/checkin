<%@ Page Title="Recover Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RecoverPassword.aspx.cs" Inherits="MovieMatrix.ASP.Account.RecoverPassword" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
    </hgroup>
    <section id="loginForm">
        <asp:PasswordRecovery ID="passwordRecovery" runat="server" />
        
    </section>
</asp:Content>
