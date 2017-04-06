<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="IEEECheckin.ASPDocs.About" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .col-custom-offset {
            margin-left: 25.0%;
        }
        .col-custom {
            width: 50.0%;
        }
        .link-override {
            color: rgba(0, 0, 0, 0.8);
            font-weight: bold;
        }
        .link-override:hover {
            text-decoration: underline;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
</asp:Content>
<asp:Content ID="JavaScriptContent" ContentPlaceHolderID="JavaScripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            var bc = $.cookie("body-color"); // text color
            if (bc != null && bc != undefined) {
                $(".link-override").css("color", "#" + bc);
            }
            else {
                $(".link-override").css("color", "rgba(0, 0, 0, 0.8)");
            }
        });
    </script>
</asp:Content>
