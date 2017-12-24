<%@ Page ValidateRequest="false" Title="Lab1" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lab1.aspx.cs" Inherits="IS7024Labs.Labs.Lab1" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Welcome to the IS 7024 Lab codebase.</h2>

    <p>Labs will show up weekly in the instructors' GITHUB account.   Run the lab as it becomes available to you.   Modifications should be in your local repository.</p>
 

    <table>
        <tr>
            <td style="background-color: lightcyan; width: 30%; text-align: center; ">
                Lab 1
            </td>
            <td style="border: 3px solid lightcyan; padding: 20px;">
            
                      <div style="float:none;padding-top: 10px; padding-bottom: 8px;">
                       1) Enter an Address:
                        <div style="float:right;"><asp:TextBox ID="tbPlaceText" runat="server"></asp:TextBox>
                        <asp:Button ID="btnGooglePlaceAutocomplete" runat="server" OnClick="btnGooglePlaceAutocomplete_Click" Text="Call Service 1" />
                    </div>

                    </div>

                    <div style="float:none;padding-top: 10px;padding-bottom: 10px;">
                       2) Enter Place ID:    
                        <div style="float:right;  ">  <asp:TextBox ID="tbPlaceID" runat="server"></asp:TextBox>
                        <asp:Button ID="btnGooglePlaceDetails" runat="server" OnClick="btnGooglePlaceDetails_Click" Text="Call Service 2" />
                    </div>
                        </div>

        
            </td>
        </tr>

    </table>


</asp:Content>
