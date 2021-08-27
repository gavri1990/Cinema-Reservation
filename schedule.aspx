<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="schedule.aspx.cs" Inherits="Cinema_Reservation.admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Movie Schedule</title>
     <link href="login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:LinkButton ID="LinkButton1" runat="server" style = "float:right " ForeColor="White" OnClick="LinkButton1_Click">Log out</asp:LinkButton>
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
                    <asp:Timer ID="Timer1" runat="server" Enabled="true" Interval="60000" OnTick="Timer1_Tick"></asp:Timer>

        <div class="container-forms">
         <div class="login-page"style="padding: 8% 0 0;">
          <div class="form">
            <asp:Label ID="Label1" runat="server" Text="Set movie schedule" Font-Bold="True" Font-Size="Large"></asp:Label><br/><br/>
              <br/>
              <asp:DropDownList ID="DropDownList1" runat="server" style =" background: #f2f2f2; font-family: Roboto, sans-serif; font-size: 14px; margin: 0 0 15px; padding: 15px; cursor: pointer; width:99%;" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                  <asp:ListItem>Title</asp:ListItem>
                  
              </asp:DropDownList> 
              <asp:DropDownList ID="DropDownList2" runat="server" style =" background: #f2f2f2; font-family: Roboto, sans-serif; font-size: 14px; margin: 0 0 15px; padding: 15px; cursor: pointer; width:99%;">
                  <asp:ListItem>Screen</asp:ListItem>
                  <asp:ListItem Value="1">Screen 01</asp:ListItem>
                  <asp:ListItem Value="2">Screen 02</asp:ListItem>
                  <asp:ListItem Value="3">Screen 03</asp:ListItem>
                  <asp:ListItem Value="4">Screen 04</asp:ListItem>
                  <asp:ListItem Value="5">Screen 05</asp:ListItem>
                  <asp:ListItem Value="6">Screen 06</asp:ListItem>
                  <asp:ListItem Value="7">Mega Screen</asp:ListItem>
                  <asp:ListItem Value="8">Golden Hall</asp:ListItem>
                  <asp:ListItem Value="9">Dolby Atmos</asp:ListItem>
              </asp:DropDownList> 
              <asp:DropDownList ID="DropDownList3" runat="server" style =" float:left; background: #f2f2f2; font-family: Roboto, sans-serif; font-size: 14px; margin: 0 0 15px; padding: 15px; cursor: pointer; width:45%;">
                  <asp:ListItem>Start Time</asp:ListItem>
                  <asp:ListItem>18:00</asp:ListItem>
                  <asp:ListItem>18:30</asp:ListItem>
                  <asp:ListItem>19:00</asp:ListItem>
                  <asp:ListItem>19:30</asp:ListItem>
                  <asp:ListItem>20:00</asp:ListItem>
                  <asp:ListItem>20:30</asp:ListItem>
                  <asp:ListItem>21:00</asp:ListItem>
                  <asp:ListItem>21:30</asp:ListItem>
                  <asp:ListItem>22:00</asp:ListItem>
                  <asp:ListItem>22:30</asp:ListItem>
                  <asp:ListItem>23:00</asp:ListItem>
                  <asp:ListItem>23:30</asp:ListItem>
              </asp:DropDownList> 
              <asp:DropDownList ID="DropDownList4" runat="server" style =" background: #f2f2f2; font-family: Roboto, sans-serif; font-size: 14px; margin: 0 0 15px; padding: 15px; cursor: pointer; width:99%;">
                  <asp:ListItem>Screening Period</asp:ListItem>
                  <asp:ListItem>1 Day</asp:ListItem>
                  <asp:ListItem>2 Days</asp:ListItem>
                  <asp:ListItem>3 Days</asp:ListItem>
                  <asp:ListItem>1 Week</asp:ListItem>
                  <asp:ListItem>2 Weeks</asp:ListItem>
              </asp:DropDownList> 
              <br/><br/><asp:Button ID="Button1" runat="server" Text="Set Schedule" OnClick="Button1_Click" />
              <asp:Label ID="Label3" runat="server" Text="Label" Font-Size="Small" Font-Bold="True"></asp:Label><br /><br /> 
              <p class="message">Add a movie?&nbsp&nbsp<asp:HyperLink ID="HyperLink2" runat="server" Text="Click here" NavigateUrl="movies.aspx"></asp:HyperLink></p>
           </div>
        </div>
       </div>
    </form>
</body>
</html>