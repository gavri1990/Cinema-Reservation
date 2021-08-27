<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="reserveseats.aspx.cs" Inherits="Cinema_Reservation.reserveseats" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Seat Reservation</title>
    <link href="login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:LinkButton ID="LinkButton1" runat="server" style = "float:right " ForeColor="White" OnClick="LinkButton1_Click">Log out</asp:LinkButton>
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
                    <asp:Timer ID="Timer1" runat="server" Enabled="true" Interval="60000" OnTick="Timer1_Tick"></asp:Timer>
				
        <div class="login-pagetickets" style="float:left">
          <div class="formtickets" style="margin-left:150px">            
              <asp:Panel ID="Panel1" runat="server"></asp:Panel>
                  
           </div>
        </div>

        <div class="login-page" style="float:left">
          <div class="form">    
              <asp:Panel ID="Panel2" runat="server" style="text-align:left">
                  <asp:Label ID="Label3" runat="server" Text="Movie:" ForeColor="Orange" Font-Bold="True" style="float:left; margin-left:8px"></asp:Label>&nbsp&nbsp<asp:Label ID="Label4" runat="server" Text="" ForeColor="Black"></asp:Label><br/>
                  <asp:Label ID="Label5" runat="server" Text="Time:" ForeColor="Orange" Font-Bold="True" style="float:left; margin-left:14px"></asp:Label>&nbsp&nbsp<asp:Label ID="Label6" runat="server" Text="" ForeColor="Black"></asp:Label><br/>
                  <asp:Label ID="Label7" runat="server" Text="Tickets:" ForeColor="Orange" Font-Bold="True" style="float:left"></asp:Label>&nbsp&nbsp<asp:Label ID="Label8" runat="server" Text="0" ForeColor="Black"></asp:Label><br/>
                  <asp:Label ID="Label9" runat="server" Text="Total:" ForeColor="Orange" Font-Bold="True" style="float:left; margin-left:14px"></asp:Label>&nbsp&nbsp<asp:Label ID="Label10" runat="server" Text="€0" ForeColor="Black"></asp:Label><br/>
                  <asp:Label ID="Label11" runat="server" Text="Seats:" ForeColor="Orange" Font-Bold="True" style="float:left; margin-left:10px"></asp:Label>&nbsp&nbsp<asp:Label ID="Label12" runat="server" Text="" ForeColor="Black"></asp:Label><br/><br/>
                  <p><asp:Button ID="Button1" runat="server" Text="Book Now" BackColor="#666666" ForeColor="White" style="cursor:pointer" OnClick="Button1_Click"/></p>
                  <asp:Label ID="Label1" runat="server" Text="" Font-Size="Small" ForeColor="Red" Font-Bold="True"></asp:Label>
              </asp:Panel>
           </div>
        </div>
    </form>
</body>
</html>