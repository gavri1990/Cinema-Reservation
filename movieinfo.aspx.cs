using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;
using System.Web.Http;
using System.Net;

namespace Cinema_Reservation
{
    public partial class movieinfo : System.Web.UI.Page
    {
        Model1Container1 context = new Model1Container1();//δημιουργία object της class ModelContainer για σύνδεση με ΒΔ
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

            Label12.Visible = false;

            if(!IsPostBack) //αν φορτώνει πρώτη φορά, όχι μέσω redirect
            {
                screeningsdiv.Visible = false; //αρχικά κρυμμένο το div με τις προγραμματισμένες προβολές
            }           

            int chosenMovieId = Convert.ToInt32(Session["chosenMovieId"]); //παίρνουμε το id της επιλεγμένης σε προηγ. φόρμα ταινίας

            try
            {
                var query = from m in context.MovieSet //βρίσκουμε την ταινία με βάση id επιλογής σε προηγούμενη φόρμα
                            where m.Id == chosenMovieId
                            select new { m.Title, m.Year, m.Duration, m.AgeRating, m.Director, m.Actor1, m.Actor2, m.Actor3, m.ImageUrl };

                foreach(var item in query)
                {
                    if (!string.IsNullOrWhiteSpace(item.ImageUrl)) //αν υπάρχει φωτογραφία για την ταινία
                    {
                        photodiv.Style["background-image"] = Page.ResolveUrl("~/MovieImages//" + item.ImageUrl); //εύρεση φωτό από φάκελο project με βάση όνομα αρχείου αποθηκευμένο σε ΒΔ
                    }

                    Label1.Text = item.Title;
                    Label3.Text = item.Duration + "'";
                    Label5.Text = item.Year;
                    Label7.Text = item.Director;
                    Label9.Text = item.Actor1;

                    if(!string.IsNullOrWhiteSpace(item.Actor2)) //αν έχει περαστεί όνομα 2ου ηθοποιού σε ΒΔ
                    {
                        Label9.Text += ", " + item.Actor2;
                    }
                    if (!string.IsNullOrWhiteSpace(item.Actor3)) //αν έχει περαστεί όνομα 3ου ηθοποιού σε ΒΔ
                    {
                        Label9.Text += ", " + item.Actor3;
                    }
                    Label11.Text = item.AgeRating;
                }
                            
            }
            catch(Exception exc)
            {
                Label12.Visible = true;
            }

            try
            {
                
                var query = (from sc in context.ScreeningSet
                             join s in context.ScreenSet on sc.Screen.Id equals s.Id
                             where sc.Movie.Id == chosenMovieId
                             select new { s.Name, sc.StartTime, sc.EndTime})
                             .GroupBy(x => new { x.Name, x.StartTime, x.EndTime })
                             .Select (y => new { y.Key.Name, y.Key.StartTime, y.Key.EndTime }); //ανάκτηση από ΒΔ αιθουσών όπου προβάλλεται η ταινία

                foreach(var item in query)
                {
                    HtmlGenericControl firstDiv = new HtmlGenericControl("div");
                    firstDiv.Attributes["class"] = "col-md-6 col-lg-4";

                    HtmlGenericControl secDiv = new HtmlGenericControl("div");
                    secDiv.Attributes["class"] = "block-33";

                    HtmlGenericControl thirdDiv = new HtmlGenericControl("div");
                    thirdDiv.Attributes["class"] = "vcard d-flex mb-3";

                    HtmlGenericControl fourthDiv = new HtmlGenericControl("div");
                    fourthDiv.Attributes["class"] = "name-text align-self-center";

                    Label label1 = new Label();
                    label1.Text = item.Name; //όνομα αίθουσας
                    label1.Style.Add("class", "heading");
                    label1.Style.Add("color", "black");
                    label1.Style.Add("font-weight", "bold");
                    label1.Style.Add("float", "left");
                    fourthDiv.Controls.Add(label1);
                    fourthDiv.Controls.Add(new LiteralControl("<br/>"));

                    DropDownList dropDownList = new DropDownList();
                    dropDownList.Style.Add("position", "absolute");
                    dropDownList.Style.Add("float", "left");
                    dropDownList.Style.Add("class", "heading");
                    dropDownList.Style.Add("color", "black");
                    dropDownList.Style.Add("background", "#f2f2f2");
                    dropDownList.Style.Add("font-family", "Roboto, sans-serif");
                    dropDownList.Style.Add("font-size", "16px");
                    dropDownList.Style.Add("cursor", "pointer");
                    dropDownList.Style.Add("border", "3px solid #999999");
                    dropDownList.Items.Add(new ListItem("Choose a date", "0"));


                    var query1 = from sc in context.ScreeningSet
                                 join s in context.ScreenSet on sc.Screen.Id equals s.Id
                                 where (sc.Movie.Id == chosenMovieId && sc.Screen.Name == item.Name && sc.StartTime == item.StartTime)
                                 select new { sc.Id, sc.Date, sc.ReservedSeats, s.Capacity }; //ανάκτηση ημερομηνιών προβολής ανά αίθουσα και ώρα προβολής

                    foreach (var item1 in query1)
                    {
                        //περνάμε το Id του κάθε screening ως (αφανή) value στην dropDownList!
                        dropDownList.Items.Add(new ListItem(item1.Date.DayOfWeek + " " + item1.Date.ToShortDateString(), item1.Id.ToString()));

                        if (item1.ReservedSeats <= item1.Capacity * 20 / 100) //έως 20% πληρότητα, πράσινο χρώμα
                        {
                            dropDownList.Items.FindByValue(item1.Id.ToString()).Attributes.Add("style", "color:green; font-weight:bold");
                        }
                        else if (item1.ReservedSeats <= item1.Capacity * 70 / 100) //από 21% έως 70% πληρότητα, πορτοκαλί χρώμα
                        {
                            dropDownList.Items.FindByValue(item1.Id.ToString()).Attributes.Add("style", "color:orange; font-weight:bold");
                        }
                        else if (item1.ReservedSeats <= item1.Capacity - 1) //από 71% πληρότητα εώς μία εναπομείνασα θέση, κόκκινο χρώμα
                        {
                            dropDownList.Items.FindByValue(item1.Id.ToString()).Attributes.Add("style", "color:red; font-weight:bold");
                        }
                        else //πλήρης αίθουσα
                        {
                            dropDownList.Items.FindByValue(item1.Id.ToString()).Enabled = false; //δεν θα εμφανίζεται στον χρήστη
                        }
                    }

                    fourthDiv.Controls.Add(dropDownList);
                    fourthDiv.Controls.Add(new LiteralControl("<br/>"));



                    Label label3 = new Label();
                    label3.Text = string.Format("{0:00}:{1:00}", item.StartTime.Hour, item.StartTime.Minute) +
                        " - " + string.Format("{0:00}:{1:00}", item.EndTime.Hour, item.EndTime.Minute); //ώρα έναρξης - λήξης
                    label3.Style.Add("class", "heading");
                    label3.Style.Add("color", "black");
                    label3.Style.Add("float", "left");
                    fourthDiv.Controls.Add(label3);
                    fourthDiv.Controls.Add(new LiteralControl("<br/><br/>"));



                    Button button = new Button();                  
                    button.Text = "Get tickets";
                    button.Style.Add("background-color", "#4D4D4D");
                    button.Style.Add("color", "white");
                    button.Style.Add("position", "relative");
                    button.Style.Add("cursor", "pointer");
                    button.Click += new EventHandler(GetTickets_Click); //δίνουμε μια μέθοδο για το click event
                    fourthDiv.Controls.Add(button);
                    fourthDiv.Controls.Add(new LiteralControl("<br/><br/>"));


                    Label label5 = new Label();
                    label5.Text = "Please choose a screening date"; //μήνυμα σε περίπτωση μη επιλογής ημερομηνίας προβολής 
                    label5.Style.Add("class", "heading");
                    label5.Style.Add("font-size", "15px");
                    label5.Style.Add("color", "red");
                    label5.Style.Add("float", "left");
                    label5.Style.Add("font-weight", "bold");
                    label5.Visible = false; //αρχικά κρυμμένο το μήνυμα, εμφάνιση υπό συνθήκες
                    fourthDiv.Controls.Add(label5);
                    


                    thirdDiv.Controls.Add(fourthDiv);
                    secDiv.Controls.Add(thirdDiv);
                    firstDiv.Controls.Add(secDiv);
                    screeningsdiv.Controls.Add(firstDiv);
                }          
            }
            catch (Exception exc)
            {
                Label12.Visible = true;
            }

        }

        protected void Button1_Click(object sender, EventArgs e) //κουμπί See scheduled screenings
        {
            int chosenMovieId = Convert.ToInt32(Session["chosenMovieId"]); //παίρνουμε το id της επιλεγμένης σε προηγ. φόρμα ταινίας
            screeningsdiv.Visible = true; //εμφάνιση κρυμμένου div με προγραμματισμένες προβολές
            HyperLink1.Focus(); //focus στο σημείο των προβολών μέσω του hyperlink         
        }

        protected void GetTickets_Click(object sender, EventArgs e) //κώδικας μεθόδου
        {
            Button pressedButton = (Button)sender; //κάνουμε cast το server object σε Button
            var dropDownDate = ((Button)sender).Parent.Controls.OfType<DropDownList>().FirstOrDefault();
            var errorLabel = ((Button)sender).Parent.Controls.OfType<Label>().Last();

            if(dropDownDate.SelectedItem.Text.Equals("Choose a date"))
            {
                errorLabel.Visible = true; //εμφάνιση label με μήνυμα "λάθους"
                HyperLink1.Focus(); //focus στο σημείο των προβολών μέσω του hyperlink
            }
            else
            {
                errorLabel.Visible = false; //απόκρυψη label με μήνυμα "λάθους"
                Session["chosenScreeningId"] = dropDownDate.SelectedValue; //αποθηκεύουμε την ID που βρίσκεται στο value του dropDownList στο Session
                Response.Redirect("reserveseats.aspx"); //μετάβαση σε φόρμα με λεπτομέρειες επιλεγμένης ταινίας
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
                        Response.Redirect("movieinfo.aspx"); //refresh σελίδας, η PageLoad θα διαβάσει τιμή παραπάνω key και θα δώσει exception
                    }
                }
                else //αν δεν έχει επιλέξει να τον 'θυμάται' η εφαρμογή, δημιουργείται μόνο jwt
                {
                    int id = Convert.ToInt32(Session["customerId"]);
                    string token = jwtMaster.GenerateTokenUser(id);
                    HttpCookie cookie = new HttpCookie("jwt", token); //δημιουργία jwt με χρήση μεθόδου class μας
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