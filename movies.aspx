<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="movies.aspx.cs" Inherits="Cinema_Reservation.movies" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Movies</title>
     <link href="login.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:LinkButton ID="LinkButton1" runat="server" style = "float:right " ForeColor="White" OnClick="LinkButton1_Click">Log out</asp:LinkButton>
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
                    <asp:Timer ID="Timer1" runat="server" Enabled="true" Interval="60000" OnTick="Timer1_Tick"></asp:Timer>

        <div class="container-forms">
         <div class="login-page" style="padding: 3% 0 0;">
          <div class="form">
            <asp:Label ID="Label1" runat="server" Text="Add movie entry" Font-Bold="True" Font-Size="Large"></asp:Label><br/><br/>
              <br/>
              <asp:TextBox ID="TextBox1" placeholder="Title" runat="server" MaxLength="80" />
              <asp:TextBox ID="TextBox2" placeholder="Duration" runat="server" MaxLength="3" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 46'/>
              <asp:TextBox ID="TextBox3" placeholder="Year" runat="server" MaxLength="4" onkeypress='return (event.charCode >= 48 && event.charCode <= 57) || event.keyCode == 8 || event.keyCode == 46'/>
              <asp:DropDownList ID="DropDownList1" runat="server" style =" background: #f2f2f2; font-family: Roboto, sans-serif; font-size: 14px; margin: 0 0 15px; padding: 15px; cursor: pointer; width:100%;">
                  <asp:ListItem>Age Rating</asp:ListItem>
                  <asp:ListItem>G</asp:ListItem>
                  <asp:ListItem>PG</asp:ListItem>
                  <asp:ListItem>PG-13</asp:ListItem>
                  <asp:ListItem>R</asp:ListItem>
                  <asp:ListItem>NC-17</asp:ListItem>
              </asp:DropDownList> 
              <asp:TextBox ID="TextBox4" placeholder="Director" runat="server" MaxLength="50" />
              <asp:TextBox ID="TextBox5" placeholder="Actor 1" runat="server" MaxLength="50" />
              <asp:TextBox ID="TextBox6" placeholder="Actor 2" runat="server" MaxLength="50" />
              <asp:TextBox ID="TextBox7" placeholder="Actor 3" runat="server" MaxLength="50" />
              <asp:FileUpload ID="FileUpload1" runat="server" accept=".jpg, .jpeg, .png"/>
              <br/><br/><asp:Button ID="Button1" runat="server" Text="Add Movie" OnClick="Button1_Click" />
              <asp:Button ID="Button2" runat="server" Text="Continue" OnClick="Button2_Click" />
              <asp:Label ID="Label3" runat="server" Text="Label" Font-Size="Small" Font-Bold="True"></asp:Label><br /><br /> 
              <p runat="server" ID="labelgray" class="message">Not adding a movie?&nbsp&nbsp<asp:HyperLink ID="HyperLink2" runat="server" Text="Proceed to schedule" NavigateUrl="schedule.aspx"></asp:HyperLink></p>
              <asp:Button ID="Button6" runat="server" Text="Yes"  BackColor="Maroon" Width ="100px" style =" float:left;" OnClick="Button6_Click"/>
              <asp:Button ID="Button7" runat="server" Text="No"  BackColor="#0066CC" Width ="100px" style =" float:right;" OnClick="Button7_Click"/>
               <asp:HyperLink ID="HyperLink1" runat="server"></asp:HyperLink> <!--για το focus-->
           </div>
        </div>
       </div>
    </form>
</body>
</html>