<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="registration.aspx.cs" Inherits="Cinema_Reservation.registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Member Registration</title>
     <link href="login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-forms">
         <div class="login-page" style="padding: 10% 0 0;">
          <div class="form">
            <asp:Label ID="Label1" runat="server" Text="Member Registration" Font-Bold="True" Font-Size="Large"></asp:Label><br/><br/>
              <asp:Label ID="Label2" runat="server" Text="Please fill the required fields below" Font-Size="Small" Font-Bold="False" Font-Italic="True"></asp:Label><br/><br/>
              <asp:TextBox ID="TextBox1" placeholder="email" runat="server" MaxLength="30" />
              <asp:TextBox ID="TextBox2" placeholder="password" runat="server" TextMode="Password" MaxLength="20" />
              <asp:Label ID="Label4" runat="server" Text="Password must be at least 6 characters long, containing at least 1 lowercase character, 1 uppercase character, 1 number and 1 special character" Font-Size="Small" Font-Bold="False" Font-Italic="False" ForeColor="Gray"></asp:Label><br/>
              <br/><br/><asp:Button ID="Button1" runat="server" Text="Submit" OnClick="Button1_Click" />
              <asp:Button ID="Button2" runat="server" Text="Continue" OnClick="Button2_Click" />
              <asp:Label ID="Label3" runat="server" Text="Label" Font-Size="Small" Font-Bold="True"></asp:Label><br /><br />            
           </div>
        </div>
       </div>
    </form>
</body>
</html>

