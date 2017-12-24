<%@ Page Title="Lab2" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lab2.aspx.cs" Inherits="IS7024Labs.Labs.Lab2" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Welcome to the IS 7024 Lab codebase.</h2>

    <p>Labs will show up weekly in the instructors' GITHUB account.   Run the lab as it becomes available to you.   Modifications should be in your local repository.</p>


    <table>
        <tr>
            <td style="border-style:solid; background-color: lightcyan; width: 30%; text-align: center;">Lab 2A -- XPath
            </td>
            <td style="border: 3px solid lightcyan; padding: 20px;">
                <div style="float: none; padding-top: 10px; padding-bottom: 8px;">
                    &nbsp;<asp:Button ID="btlLoadPeople" runat="server" Text="Load People" OnClick="btlLoadPeople_Click" />
                </div>
                <div style="float: none; padding-top: 10px; padding-bottom: 8px;">
                    &nbsp;<asp:Label ID="lblFieldReps" runat="server" Text="Field Reps"></asp:Label>
                    <br />
                    <asp:ListBox ID="lbFieldReps" runat="server" Height="173px" Width="406px"></asp:ListBox>
                </div>
                <div style="float: none; padding-top: 10px; padding-bottom: 8px;">
                    &nbsp;                                         
                    <asp:Label ID="lblClients" runat="server" Text="Clients"></asp:Label><br />
                    <asp:ListBox ID="lbClients" runat="server" Height="173px" Width="406px"></asp:ListBox>
                </div>


            </td>
        </tr>
        <tr>

            <td style="border-style:solid; background-color: lightcyan; width: 30%; text-align: center;">Lab 2B -- XSD
            </td>
            <td style="border: 3px solid lightcyan; padding: 20px;">
                <div style="float: none; padding-top: 10px; padding-bottom: 8px;">
                    <asp:Label ID="lbXSD" runat="server" Text="XSD FileName"></asp:Label>
                    &nbsp;<br />
                    <asp:TextBox ID="tbXSD" runat="server" Width="228px" Wrap="False"></asp:TextBox>
                    <asp:Button ID="btnXSD" runat="server" OnClick="btnXSD_Click" Text="Validate" />
                </div>
                <div style="float: none; padding-top: 10px; padding-bottom: 8px;">
                    &nbsp;<asp:Label ID="Label1" runat="server" Text="Results"></asp:Label>
                    <br />
                    <asp:TextBox ID="tbXSDResults" runat="server" Height="205px" ReadOnly="True" Width="396px" TextMode="MultiLine"></asp:TextBox>
                    <br />
                </div>
                <div style="float: none; padding-top: 10px; padding-bottom: 8px;">
                    &nbsp;                                         
                    <br />
                </div>


            </td>

        </tr>

    </table>


</asp:Content>

