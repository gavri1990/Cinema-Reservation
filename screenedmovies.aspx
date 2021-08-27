<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="screenedmovies.aspx.cs" Inherits="Cinema_Reservation.screenedmovies" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css">
        .inline{ display: inline; }
    </style>
    <title>Movies</title>
    <link href="style.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:LinkButton ID="LinkButton1" runat="server" style = "float:right " ForeColor="White" OnClick="LinkButton1_Click">Log out</asp:LinkButton>
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
                    <asp:Timer ID="Timer1" runat="server" Enabled="true" Interval="60000" OnTick="Timer1_Tick"></asp:Timer>

        <div class="site-section bg-light">
      <div class="container">

       
        <div class="row mb-5 pt-5 justify-content-center">
            <div class="col-md-7 text-center section-heading">
              <h2 class="heading"><asp:Label ID="Label1" runat="server" Text="Movie Selection" ForeColor="White" style="width:50%"></asp:Label>
                  <br /><asp:Label ID="Label2" runat="server" Text="Dates" Font-Size="Large" ForeColor="White" style="width:50%"></asp:Label>
              </h2>
            </div>
          </div>

        <div class="row" id="rowdiv" runat="server">
          
          
         
        </div>
      </div>
    </div>
    </form>
</body>
</html>
