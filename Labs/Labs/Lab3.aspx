<%@ Page Title="Lab3" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lab3.aspx.cs" Inherits="IS7024Labs.Labs.Lab3" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Welcome to the IS 7024 Lab codebase.</h2>

    <p>Labs will show up weekly in the instructors' GITHUB account.   Run the lab as it becomes available to you.   Modifications should be in your local repository.</p>
 

    <table>
        <tr>
            <td style="background-color: lightcyan; width: 30%; text-align: center; ">
                Lab 3
                - A
            </td>
            <td style="border: 3px solid lightcyan; padding: 20px; width: 312px;">
            
                      <div style="float:none;padding-top: 10px; padding-bottom: 8px;">
                          Enter GeoCodes (lat/long) for Location1<br />
                          <asp:TextBox ID="tbLab3ALatitude1" runat="server"></asp:TextBox>
                          <asp:TextBox ID="tbLab3ALongitude1" runat="server"></asp:TextBox>
                          <br />
                          Enter GeoCodes (lat/long) for Location2<br />
                          <asp:TextBox ID="tbLab3ALatitude2" runat="server"></asp:TextBox>
                          <asp:TextBox ID="tbLab3ALongitude2" runat="server"></asp:TextBox>
                          <br />
                          <asp:Button ID="bntInvokeA" runat="server" Text="Get Distance" OnClick="bntInvokeA_Click" />

                          <asp:TextBox ID="tbLab3ADistance" runat="server"></asp:TextBox>

                    </div>

        
            </td>
        </tr>
        <tr>
            <td style="background-color: lightcyan; width: 30%; text-align: center; ">
                Lab 3
                - B </td>
            <td style="border: 3px solid lightcyan; padding: 20px; width: 312px;">
            
                      <div style="float:none;padding-top: 10px; padding-bottom: 8px;">
                          Enter Address1<br />
                          <asp:TextBox ID="tbLab3BAddress1" runat="server" Width="304px"></asp:TextBox>
                          <br />
                          Enter Address 2<br />
                          <asp:TextBox ID="tbLab3BAddress2" runat="server" Width="300px"></asp:TextBox>
                          <br />
                          <asp:Button ID="btnInvokeB" runat="server" Text="Get Distance" OnClick="btnInvokeB_Click" />

                          <asp:TextBox ID="tbLab3BDistance" runat="server"></asp:TextBox>

                    </div>

        
            </td>
        </tr>

    </table>


</asp:Content>
