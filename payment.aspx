<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="payment.aspx.cs" Inherits="Cinema_Reservation.payment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payment Method</title>
    <link href="login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
                    
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
                    <asp:Timer ID="Timer1" runat="server" Enabled="false" Interval="5000" OnTick="Timer1_Tick" ></asp:Timer>
                    <asp:Timer ID="Timer2" runat="server" Enabled="true" Interval="60000" OnTick="Timer2_Tick"></asp:Timer>

        <div class="login-page" style="padding: 5% 0 0;">
          <div class="form">
            <asp:Label ID="Label1" runat="server" Text="Payment Method" Font-Bold="True" Font-Size="Large"></asp:Label><br/><br/>
              <asp:Label ID="Label2" runat="server" Text="Please select payment method for the total amount of:" Font-Bold="True" Font-Size="Small"></asp:Label>&nbsp<asp:Label ID="Label3" runat="server" Text="500.00" Font-Bold="True" ForeColor="#0099FF"></asp:Label>&nbsp<asp:Label ID="Label4" runat="server" Text="EUR" Font-Bold="True" ForeColor="Gray" Font-Size="Small"></asp:Label><br /><br />
              <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" style =" background: #f2f2f2; font-family: Roboto, sans-serif; font-size: 14px; margin: 0 0 15px; padding: 15px; cursor: pointer; width:100%;" >
                  <asp:ListItem>MasterCard</asp:ListItem>
                  <asp:ListItem>Visa</asp:ListItem>
                  <asp:ListItem>American Express</asp:ListItem>
                  <asp:ListItem>Diners</asp:ListItem>
              </asp:DropDownList> 
              <asp:TextBox ID="TextBox1" placeholder="credit card number" runat="server" MaxLength="16" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 46'/>
              <asp:TextBox ID="TextBox2" placeholder="cardholder name" runat="server" MaxLength="50" onkeypress='return (event.charCode >= 65 && event.charCode <= 90) || (event.charCode >= 97 && event.charCode <= 122) || event.keyCode == 8 || event.keyCode == 46 || event.keyCode == 32'/>
              <asp:Label ID="Label5" runat="server" Text="Expiration date and security code" Font-Bold="True" Font-Size="Small" ForeColor="Gray"></asp:Label><br /><br />
              <asp:DropDownList ID="DropDownList2" runat="server" style =" background: #f2f2f2; font-family: Roboto, sans-serif; font-size: 14px; margin: 0 0 15px; padding: 15px; cursor: pointer; width:27%;">
                  <asp:ListItem>- -</asp:ListItem>
                  <asp:ListItem>01</asp:ListItem>
                  <asp:ListItem>02</asp:ListItem>
                  <asp:ListItem>03</asp:ListItem>
                  <asp:ListItem>04</asp:ListItem>
                  <asp:ListItem>05</asp:ListItem>
                  <asp:ListItem>06</asp:ListItem>
                  <asp:ListItem>07</asp:ListItem>
                  <asp:ListItem>08</asp:ListItem>
                  <asp:ListItem>09</asp:ListItem>
                  <asp:ListItem>10</asp:ListItem>
                  <asp:ListItem>11</asp:ListItem>
                  <asp:ListItem>12</asp:ListItem>
              </asp:DropDownList>&nbsp
              <asp:DropDownList ID="DropDownList3" runat="server" style =" background: #f2f2f2; font-family: Roboto, sans-serif; font-size: 14px; margin: 0 0 15px; padding: 15px; cursor: pointer; width:35%;">
                  <asp:ListItem>- - - -</asp:ListItem>
                  <asp:ListItem>2020</asp:ListItem>
                  <asp:ListItem>2021</asp:ListItem>
                  <asp:ListItem>2022</asp:ListItem>
                  <asp:ListItem>2023</asp:ListItem>
                  <asp:ListItem>2024</asp:ListItem>
                  <asp:ListItem>2025</asp:ListItem>
                  <asp:ListItem>2026</asp:ListItem>
                  <asp:ListItem>2027</asp:ListItem>
                  <asp:ListItem>2028</asp:ListItem>
                  <asp:ListItem>2029</asp:ListItem>
                  <asp:ListItem>2030</asp:ListItem>
              </asp:DropDownList>&nbsp<asp:TextBox ID="TextBox3" placeholder="CVV" runat="server" style =" width:30%;" TextMode="Password" MaxLength="3" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 46'/><br/><br/>
              <asp:Button ID="Button1" runat="server" Text="Proceed with payment" OnClick="Button1_Click" />
              <asp:Label ID="Label6" runat="server" Text="Label" Font-Bold="True" Font-Size="Small"></asp:Label><br/><br/><br />
              <asp:Image ID="Image1" runat="server" ImageUrl="~/loading.gif" Height="70px" Width="70px" />
              <asp:Button ID="Button2" runat="server" Text="Continue" OnClick="Button2_Click" />    
              <asp:HyperLink ID="HyperLink1" runat="server"></asp:HyperLink> <!--για το focus-->
           </div>
        </div>
    </form>
</body>
</html>
