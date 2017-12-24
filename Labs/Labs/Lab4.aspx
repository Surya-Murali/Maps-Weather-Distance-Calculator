<%@ Page Title="Lab4" validateRequest="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lab4.aspx.cs" Inherits="IS7024Labs.Labs.Lab4" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Welcome to the IS 7024 Lab codebase.</h2>

    <p>Labs will show up weekly in the instructors' GITHUB account.   Run the lab as it becomes available to you.   Modifications should be in your local repository.</p>
 

    <table>
        <tr>
            <td style="background-color: lightcyan; width: 30%; text-align: center; ">
                Lab 4
            </td>
            <td style="border: 3px solid lightcyan; padding: 20px;">
                      <div style="float:none;padding-top: 10px; padding-bottom: 8px;">
                          <asp:Button ID="btnGetPeople" runat="server" OnClick="btnGetPeople_Click" Text="Get People" />
                          <br />
                          <asp:ListBox ID="lbPeople" runat="server" Height="96px" Width="195px"></asp:ListBox>
                          <asp:Button ID="btnEdit" runat="server" OnClick="btnEdit_Click" Text="Edit Selected" />
                          <br />
                          <br />
                          <br />
                          <asp:TextBox ID="tbEditPerson" runat="server" Height="204px" TextMode="MultiLine" Width="420px" Wrap="False"></asp:TextBox>
                          <br />
                          <asp:Button ID="btnNew" runat="server" Text="New" Width="94px" OnClick="btnNew_Click" />
                          <asp:Button ID="btnSave0" runat="server" OnClick="btnSave_Click" Text="Save" Width="94px" />
                          <br />
                          <br />
                          <br />
                          <asp:Label ID="lblStatus" runat="server" ForeColor="Blue"></asp:Label>
                        </div>
            </td>
        </tr>

    </table>


</asp:Content>