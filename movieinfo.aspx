<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="movieinfo.aspx.cs" Inherits="Cinema_Reservation.movieinfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css">
        .inline{ display: inline; }
    </style>
    <title>Screenings</title>
    <link href="style.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:LinkButton ID="LinkButton1" runat="server" style = "float:right " ForeColor="White" OnClick="LinkButton1_Click">Log out</asp:LinkButton>
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
                    <asp:Timer ID="Timer1" runat="server" Enabled="true" Interval="60000" OnTick="Timer1_Tick"></asp:Timer>

    
    <div class="site-section bg-light" style="margin-top: 5%">
      <div class="container">
        <div class="row mb-5">
          <div class="col-md-12 mb-5">
            
            <div class="block-3 d-md-flex">
              <div class="image" id="photodiv" runat="server" style="background-image: url('MovieImages//filmReel.jpg'); ">
              </div>
              <div class="text">
                <h2 class="heading"><asp:Label ID="Label1" runat="server" Text="Movie Title" ForeColor="Black"></asp:Label></h2>
                <ul class="specs mb-5">
                  <asp:Label ID="Label2" runat="server" Text="Duration:" Font-Bold="True"></asp:Label>&nbsp&nbsp<asp:Label ID="Label3" runat="server" Text=""></asp:Label><br />
                  <asp:Label ID="Label4" runat="server" Text="Year:" Font-Bold="True"></asp:Label>&nbsp&nbsp<asp:Label ID="Label5" runat="server" Text=""></asp:Label><br />
                  <asp:Label ID="Label6" runat="server" Text="Director:" Font-Bold="True"></asp:Label>&nbsp&nbsp<asp:Label ID="Label7" runat="server" Text=""></asp:Label><br />
                  <asp:Label ID="Label8" runat="server" Text="Cast:" Font-Bold="True"></asp:Label>&nbsp&nbsp<asp:Label ID="Label9" runat="server" Text=""></asp:Label><br />
                  <asp:Label ID="Label10" runat="server" Text="Rating:" Font-Bold="True"></asp:Label>&nbsp&nbsp<asp:Label ID="Label11" runat="server" Text=""></asp:Label><br />
                </ul><br /><br />

                <p><asp:Button ID="Button1" runat="server" Text="See scheduled screenings" BackColor="#4D4D4D" ForeColor="White" OnClick="Button1_Click" style="cursor:pointer"/></p>
                <br /><asp:Label ID="Label12" runat="server" Text="Something went wrong! Please try reloading the page" Font-Bold="True" ForeColor="Red"></asp:Label>

              </div>
            </div>

          </div>  
        </div>

      </div>
    </div>

  
    <div class="site-section bg-light">
      <div class="container">
        <div class="row mb-5">
        </div>
          
        <div class="row" id="screeningsdiv" runat="server">  
            <!--εδώ θα μπουν δυναμικά  τα controls-->

            
            
        </div>
      </div>
    </div>
        <asp:HyperLink ID="HyperLink1" runat="server"></asp:HyperLink> <!--για το focus-->
    </form>
</body>
</html>












