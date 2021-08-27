using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Timers;
using System.Security.Cryptography;
using System.Web.Http;
using System.Net;

namespace Cinema_Reservation
{
    public partial class payment : System.Web.UI.Page
    {
        Model1Container1 context = new Model1Container1(); //δημιουργία object της class ModelContainer για σύνδεση με ΒΔ
        private JWTAuth jwtMaster = new JWTAuth();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["throwUnauthorizedExc"] != null)
            {
                bool throwUnauthorizedExc = Convert.ToBoolean(Session["throwUnauthorizedExc"]);
                if (throwUnauthorizedExc == true)
                {
                    throwUnauthorizedExc = false;
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                }
            }

            Image1.Visible = false;
            Label6.Visible = false;
            Button2.Visible = false;

            Label3.Text = (string)Session["totalAmount"]; //συνολικό ποσό πληρωμής
        }
      
        protected void Button1_Click(object sender, EventArgs e) //κουμπί Proceed with payment
        {
            if (string.IsNullOrWhiteSpace(TextBox1.Text) || string.IsNullOrWhiteSpace(TextBox2.Text))
            {
                Label6.Visible = true;
                TextBox1.Visible = true;
                TextBox2.Visible = true;
                Label6.ForeColor = Color.Red;
                Label6.Text = "Please fill all required fields";
            }
            else if (TextBox1.Text.Length < 16)
            {
                Label6.Visible = true;
                TextBox1.Visible = true;
                TextBox2.Visible = true;
                Label6.ForeColor = Color.Red;
                Label6.Text = "Invalid credit card number (16 digits required)";
            }
            else if (TextBox3.Text.Length < 3 && TextBox3.Text.Length > 0)
            {
                Label6.Visible = true;
                TextBox1.Visible = true;
                TextBox2.Visible = true;
                Label6.ForeColor = Color.Red;
                Label6.Text = "Invalid security code (3 digits required)";
            }
            else if (DropDownList2.Text.Equals("- -") || DropDownList3.Text.Equals("- - - -"))
            {
                Label6.Visible = true;
                TextBox1.Visible = true;
                TextBox2.Visible = true;
                Label6.ForeColor = Color.Red;
                Label6.Text = "Please set expiration date";
            }
            else if (string.IsNullOrWhiteSpace(TextBox3.Text))
            {
                Label6.Visible = true;
                TextBox1.Visible = true;
                TextBox2.Visible = true;
                Label6.ForeColor = Color.Red;
                Label6.Text = "Please fill security code";
            }
            else
            {
                //φέρνουμε τα απαραίτητα δεδομένα
                int customerId = Convert.ToInt32(Session["customerId"]);
                int chosenScreeningId = Convert.ToInt32(Session["chosenScreeningId"]);
                List<string> chosenSeatIds = (List<string>)(Session["chosenSeatIds"]);
                int seatCount = Convert.ToInt16(Session["seatCount"]);
                DateTime dateFor = Convert.ToDateTime(Session["dateFor"]);


                Reservation reservation = new Reservation(); //φτιάχνουμε νέα καταχώριση για πίνακα Reservation
                reservation.DateMade = DateTime.Now;
                reservation.DateFor = dateFor;
                reservation.SeatCount = Convert.ToInt16(seatCount); //μετατροπή απο int σε int16

                try
                {                    
                    var query2 = from screening in context.ScreeningSet
                                 where screening.Id == chosenScreeningId
                                 select screening;

                    for (int i = 0; i < chosenSeatIds.Count(); i++)
                    {
                        int seatId = Convert.ToInt32(chosenSeatIds[i]); //μετατροπή από string σε int

                        ReservedSeat rSeat = new ReservedSeat(); //νέα εγγραφή reservedSeat

                        reservation.ReservedSeat.Add(rSeat); //σύνδεση Reservation - ReservedSeat


                        var query1 = from seat in context.SeatSet
                                     where seat.Id == seatId
                                     select seat;

                        foreach (Seat seat in query1)
                        {
                            seat.ReservedSeat.Add(rSeat); //σύνδεση Seat - ReserverSeat
                        }

                        foreach (Screening screening in query2) //χρήση παραπάνω ορισμένου query2
                        {
                            screening.ReservedSeat.Add(rSeat); //σύνδεση Screening - ReserverSeat
                            screening.ReservedSeats += 1; //αύξηση συνολικά κρατημένων θέσεων προβολής κατά μία
                        }
                    }

                    var query = from cust in context.CustomerSet //ανάκτηση εγγραφής πελάτη από ΒΔ
                                where cust.Id == customerId
                                select cust;

                    foreach (Customer customer in query)
                    {
                        customer.Reservation.Add(reservation); //σύνδεση reservation με customer
                    }
                    context.SaveChanges(); //αποθήκευση αλλαγών


                    Label6.Visible = true;
                    Label6.ForeColor = Color.Gray;
                    Label6.Font.Size = 12;
                    Label6.Text = "Processing payment...";
                    Button1.Visible = false;
                    Image1.Visible = true;
                    DropDownList1.Enabled = false;
                    DropDownList2.Enabled = false;
                    DropDownList3.Enabled = false;
                    TextBox1.Enabled = false;
                    TextBox2.Enabled = false;
                    TextBox3.Enabled = false;
                    Timer1.Enabled = true;
                    HyperLink1.Focus();
                }
                catch (Exception exc)
                {
                    TextBox1.Visible = false;
                    TextBox2.Visible = false;
                    Image1.Visible = false;
                    Label6.Visible = true;
                    Label6.ForeColor = Color.Red;
                    Label6.Text = "Payment failed. Please try again";
                    Button2.Visible = false;
                }
            }
        }
                  

        protected void Button2_Click(object sender, EventArgs e) //κουμπί Continue
        {
            Response.Redirect("screenedmovies.aspx");
        }

        protected void Timer1_Tick(object sender, EventArgs e) //σε tick του timer με εφέ processing payment
        {
            Button2.Visible = true;
            Button1.Visible = false;
            Image1.Visible = false;
            DropDownList1.Enabled = false;
            DropDownList2.Enabled = false;
            DropDownList3.Enabled = false;
            TextBox1.Enabled = false;
            TextBox2.Enabled = false;
            TextBox3.Enabled = false;
            Label6.Visible = true;
            Label6.ForeColor = Color.Green;
            Label6.Font.Size = 10;
            Label6.Text = "Payment complete! Tickets reserved! Enjoy the movie!";
            Button2.Focus();
            Timer1.Enabled = false;
        }

        protected void Timer2_Tick(object sender, EventArgs e)
        {
            if (Request.Cookies["jwt"] == null) //αν το jwt expired
            {
                bool remember = Convert.ToBoolean(Session["remember"]); //ανάκτηση από Session του value του key 'remember'

                if (remember == true) //αν true, ο χρήστης είχε κάνει tick στο checkbox ώστε να τον 'θυμάται' η εφαρμογή
                {
                    string incomingRefresh = Server.HtmlEncode(Request.Cookies["refreshCookie"].Value);

                    //σύγκριση τελευταίου τμήματος refresh cookie με id και του προηγούμενου με αποθ. σε ΒΔ τμήμα refresh cookie
                    var query = from c in context.CustomerSet
                                where c.Id.ToString().Equals(incomingRefresh.Substring(44))
                                        && c.RefreshToken.Equals(incomingRefresh.Substring(0, 44))
                                select c;

                    if (query.Any()) //αν υπάρχει σε στήλη της ΒΔ το refresh, δεν χρησιμοποιείται 2η φορά από κάποιον
                    {
                        foreach (var cust in query)
                        {
                            byte[] saltedCookie;
                            var getSalt = new RNGCryptoServiceProvider();
                            getSalt.GetBytes(saltedCookie = new byte[32]); //καταχώριση τυχαίας αξίας 32 bytes στο salt

                            string newSaltedCookie = Convert.ToBase64String(saltedCookie);
                            cust.RefreshToken = newSaltedCookie; //αποθήκευση τιμής νέου refreshtoken σε ΒΔ

                            //νέο refresh cookie
                            HttpCookie refreshCookie = new HttpCookie("refreshCookie", newSaltedCookie + cust.Id.ToString());
                            refreshCookie.HttpOnly = true; //ρυθμίσεις για περαιτέρω ασφάλεια cookie
                            refreshCookie.Secure = true;
                            refreshCookie.SameSite = SameSiteMode.Lax;
                            refreshCookie.Expires = DateTime.Now.AddDays(5);
                            Response.Cookies.Add(refreshCookie); //προσθήκη του στο Response του server προς τον browser

                            //νέο jwt token σε cookie
                            int id = Convert.ToInt32(Session["customerId"]);
                            string token = jwtMaster.GenerateTokenUser(id);
                            HttpCookie cookie = new HttpCookie("jwt", token);
                            cookie.HttpOnly = true;
                            cookie.Secure = true;
                            cookie.SameSite = SameSiteMode.Lax;
                            cookie.Expires = DateTime.Now.AddMinutes(5);
                            Response.Cookies.Add(cookie);
                        }
                        context.SaveChanges(); //αποθήκευση αλλαγών στη ΒΔ
                    }
                    else //αν δεν υπάρχει σε στήλη της ΒΔ το refresh, χρησιμοποιείται 2η φορά από κάποιον
                    {
                        HttpCookie refreshCookie = new HttpCookie("refreshCookie"); //διαγραφή refresh cookie από browser
                        refreshCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(refreshCookie);

                        Session["throwUnauthorizedExc"] = true; //τίθεται τιμή true στο αντίστοιχο key του Session
                        Response.Redirect("payment.aspx"); //refresh σελίδας, η PageLoad θα διαβάσει τιμή παραπάνω key και θα δώσει exception
                    }
                }
                else //αν δεν έχει επιλέξει να τον 'θυμάται' η εφαρμογή, δημιουργείται μόνο jwt
                {
                    int id = Convert.ToInt32(Session["customerId"]);
                    string token = jwtMaster.GenerateTokenUser(id); //δημιουργία jwt με χρήση μεθόδου class μας
                    HttpCookie cookie = new HttpCookie("jwt", token);
                    cookie.HttpOnly = true;
                    cookie.Secure = true;
                    cookie.SameSite = SameSiteMode.Lax;
                    cookie.Expires = DateTime.Now.AddMinutes(5);
                    Response.Cookies.Add(cookie);
                }
            }
        }
    }
}