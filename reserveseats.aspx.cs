using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cinema_Reservation
{
    public partial class reserveseats : System.Web.UI.Page
    {
        Model1Container1 context = new Model1Container1(); //δημιουργία object της class ModelContainer για σύνδεση με ΒΔ
        private JWTAuth jwtMaster = new JWTAuth();
        double ticketPrice; //global μεταβλητές
        DateTime dateFor;
        static List<string> chosenSeats = new List<string>(); //static, αλλιώς σε κάθε reload αδειάζουν
        static List<string> chosenSeatIds = new List<string>();

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

            if (!IsPostBack) //αν ερχόμαστε από άλλη φόρμα
            {
                chosenSeats.Clear(); //καθαρισμός static λιστών, πιθανώς να έχουν κρατήσει στοιχεία
                chosenSeatIds.Clear();
            }

            Label1.Visible = false;
            int chosenScreeningId = Convert.ToInt32(Session["chosenScreeningId"]);

            try
            {
                List<string> reservedSeats = new List<string>();

                var query = from sc in context.ScreeningSet
                            join s in context.ScreenSet on sc.Screen.Id equals s.Id
                            join se in context.SeatSet on s.Id equals se.Screen.Id
                            where sc.Id == chosenScreeningId
                            &&
                            (from r in context.ReservedSeatSet
                             where r.Screening.Id == chosenScreeningId
                             select r.Seat.Id)
                             .Contains(se.Id)
                           select new { se.Number };


                foreach(var item in query)
                {
                    reservedSeats.Add(item.Number); //προσθήκη κρατημένων θέσεων σε list
                }


                //query για επιλογή όλων των θέσεων της συγκεκριμένης προβολής και άλλων πληροφοριών 
                var query1 = from sc in context.ScreeningSet
                             join s in context.ScreenSet on sc.Screen.Id equals s.Id
                             join se in context.SeatSet on s.Id equals se.Screen.Id
                             where sc.Id == chosenScreeningId
                             select new { sc.Movie.Title, sc.Date, sc.StartTime, se.Id, se.Number, s.TicketPrice };


                Button screenButton = new Button();
                screenButton.Enabled = false;
                screenButton.Text = "Screen";
                screenButton.Style.Add("width", "75%");
                screenButton.Style.Add("height", "10%");
                screenButton.Style.Add("background-color", "orange");
                screenButton.Style.Add("cursor", "default");
                Panel1.Controls.Add(screenButton);
                Panel1.Controls.Add(new LiteralControl("<br/><br/>"));


                int i = 1;
                foreach(var item1 in query1)
                {
                    if (i == 1) //1 φορά θέλουμε να γίνει
                    {
                        Label4.Text = item1.Title;
                        Label6.Text = item1.Date.DayOfWeek + " " + item1.Date.ToShortDateString() +
                            " " + string.Format("{0:00}:{1:00}", item1.StartTime.Hour, item1.StartTime.Minute);
                        ticketPrice = item1.TicketPrice;
                        dateFor = item1.Date;
                    }

                    Button seatButton = new Button();
                    seatButton.ID = item1.Id.ToString(); //περνάμε το Id της κάθε θέσης ως ID στο αντίστοιχο button 
                    seatButton.Text = item1.Number;
                    seatButton.Style.Add("padding", "12px");
                    seatButton.Style.Add("font-size", "12px");
                    seatButton.Style.Add("width", "40px");
                    seatButton.Style.Add("height", "40px");
                    seatButton.Style.Add("margin-right", "1%");
                    seatButton.Click += new EventHandler(GetSeat_Click); //δίνουμε μια μέθοδο για το click event

                    if (reservedSeats.Contains(item1.Number)) //αν η θέση βρίσκεται στη λίστα με κρατημένες θέσεις
                    {
                        //μη επιλέξιμη
                        seatButton.Enabled = false;
                        seatButton.Style.Add("background-color", "red");
                        seatButton.Style.Add("cursor", "default");
                    }
                    else
                    {
                        seatButton.Style.Add("background-color", "grey");
                        seatButton.Style.Add("cursor", "pointer");
                    }                   
                    Panel1.Controls.Add(seatButton);

                    if (i % 10 == 0) //αν πολλαπλάσιο του 10 ο αριθμός θέσης, αλλαγή σειράς
                    {
                        Panel1.Controls.Add(new LiteralControl("<br/>"));
                    }
                    i++;
                }

                Panel1.Controls.Add(new LiteralControl("<br/><br/><br/>"));

                Button button = new Button();
                button.Enabled = false;
                button.Style.Add("margin-right", "3px");
                button.Style.Add("background-color", "#999999");
                button.Style.Add("float", "left");
                button.Style.Add("height", "5px");
                button.Style.Add("width", "10px");
                button.Style.Add("cursor", "default");
                Panel1.Controls.Add(button);


                Label label = new Label();
                label.Text = "Available";
                label.Style.Add("float", "left");
                label.Style.Add("margin-right", "10%");
                label.Style.Add("font-size", "12px");
                Panel1.Controls.Add(label);


                Button button1 = new Button();
                button1.Enabled = false;
                button1.Style.Add("margin-right", "3px");
                button1.Style.Add("background-color", "red");               
                button1.Style.Add("float", "left");
                button1.Style.Add("height", "5px");
                button1.Style.Add("width", "10px");
                button1.Style.Add("cursor", "default");
                Panel1.Controls.Add(button1);


                Label label1 = new Label();
                label1.Text = "Reserved";
                label1.Style.Add("float", "left");
                label1.Style.Add("margin-right", "10%");
                label1.Style.Add("font-size", "12px");
                Panel1.Controls.Add(label1);

                Button button2 = new Button();
                button2.Enabled = false;
                button2.Style.Add("margin-right", "3px");
                button2.Style.Add("background-color", "green");
                button2.Style.Add("float", "left");
                button2.Style.Add("height", "5px");
                button2.Style.Add("width", "10px");
                button2.Style.Add("cursor", "default");
                Panel1.Controls.Add(button2);


                Label label2 = new Label();
                label2.Text = "Selected";
                label2.Style.Add("float", "left");
                label2.Style.Add("font-size", "12px");
                Panel1.Controls.Add(label2);


            }
            catch(Exception exc)
            {
                Label1.Visible = true;
                Label1.Text = "Something went wrong! Please try reloading the page";
            }

        }

        protected void GetSeat_Click(object sender, EventArgs e) //κώδικας μεθόδου
        {           
            Button pressedButton = (Button)sender; //κάνουμε cast το server object σε Button
            Label12.Text = "";
            string priceNumber = Label10.Text.Substring(1); //παραλείπουμε 1ον χαρακτήρα(θέση 0) έως τελευταία

            if (!chosenSeats.Contains(pressedButton.Text)) //αν δεν έχει επιλεγεί ήδη η θέση από τον πελάτη
            {
                pressedButton.Style.Add("background-color", "green");
                chosenSeats.Add(pressedButton.Text); //βάζουμε το νούμερο κάθε επιλεμένης θέσης σε μία list  
                chosenSeatIds.Add(pressedButton.ID); //βάζουμε το ID κάθε επιλεμένης θέσης σε μία άλλη list  
                Label10.Text = "€" + (Convert.ToDouble(priceNumber) + ticketPrice).ToString();
                foreach(string element in chosenSeats)
                {
                    Label12.Text += element + " ";
                }
            } 
            else
            {
                pressedButton.Style.Add("background-color", "#999999");
                chosenSeats.Remove(pressedButton.Text); //βγάζουμε την αποεπιλεμένη θέση από την list, ψάχνοντας μέσω value 
                chosenSeatIds.Remove(pressedButton.ID); //βγάζουμε την ID αποεπιλεμένης θέσης από την list, ψάχνοντας by value 
                Label10.Text = "€" + (Convert.ToDouble(priceNumber) - ticketPrice).ToString();
                foreach (string element in chosenSeats)
                {
                    Label12.Text += element + " ";
                }
            }
            Label8.Text = chosenSeats.Count().ToString(); //εκτός if-else
        }

        protected void Button1_Click(object sender, EventArgs e) //κουμπί Book Now
        {
            if (Label12.Text.Equals("")) //αν δεν έχει επιλεχθεί καμία θέση
            {
                Label1.Text = "Please select a seat";
                Label1.Visible = true;
            }
            else
            {
                //αποθήκευση απαραίτητων πληροφοριών στο Session και μετάβαση στη σελίδα Payment
                Session["chosenSeatIds"] = chosenSeatIds;
                Session["seatCount"] = Label8.Text;
                Session["dateFor"] = dateFor;
                Session["totalAmount"] = Label10.Text.Substring(1);
                Response.Redirect("payment.aspx");
            }            
        }

        protected void Timer1_Tick(object sender, EventArgs e)
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
                        Response.Redirect("reserveseats.aspx"); //refresh σελίδας, η PageLoad θα διαβάσει τιμή παραπάνω key και θα δώσει exception
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

        protected void LinkButton1_Click(object sender, EventArgs e) //κλικ κουμπιού logout
        {
            //διαγραφή jwt και refresh cookie σε browser
            if (Request.Cookies["jwt"] != null)
            {
                HttpCookie cookie = new HttpCookie("jwt");
                cookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cookie);
            }

            HttpCookie refreshCookie = new HttpCookie("refreshCookie");
            refreshCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(refreshCookie);

            string incomingRefresh = Server.HtmlEncode(Request.Cookies["refreshCookie"].Value);

            //σύγκριση τελευταίου τμήματος refresh cookie με id και του προηγούμενου με αποθ. σε ΒΔ τμήμα refresh cookie
            var query = from c in context.CustomerSet
                        where c.Id.ToString().Equals(incomingRefresh.Substring(44))
                                && c.RefreshToken.Equals(incomingRefresh.Substring(0, 44))
                        select c;

            if (query.Any())
            {
                foreach (var cust in query)
                {
                    cust.RefreshToken = null; //null σε πεδίο ΒΔ με refresh token, ώστε να μην γίνει δεκτό αν χρησιμ. 2η φορά από επιτεθέμενο
                }
            }
            context.SaveChanges(); //αποθήκευση αλλαγών στη ΒΔ
            Response.Redirect("login.aspx"); //επιστροφή στη login
        }
    }
}