<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Cinema_Reservation.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <link href="login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
				
        <div class="login-page" style="padding: 10% 0 0;">
          <div class="form">
             <asp:TextBox ID="TextBox1" placeholder="email" runat="server" MaxLength="30" />
             <asp:TextBox ID="TextBox2" placeholder="password" runat="server" TextMode="Password" MaxLength="30" />
             <asp:Label ID="Label1" runat="server" Text="Incorrect email and/or password" Font-Size="Small" ForeColor="Red" Font-Bold="True"></asp:Label>
             <br/><br /><asp:Button ID="Button1" runat="server" Text="Login" OnClick="Button1_Click" />
             <asp:CheckBox ID="CheckBox1" runat="server" Text="Remember me" TextAlign="Left" Font-Size="Small" style="white-space: nowrap; margin-left: 20px"/>
            <p class="message">Not registered?&nbsp&nbsp<asp:HyperLink ID="HyperLink1" runat="server" Text="Create an account" NavigateUrl="registration.aspx"></asp:HyperLink>
            </p>
           </div>
        </div>
    </form>
</body>
</html>