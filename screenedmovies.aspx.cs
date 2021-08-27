using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Http;
using System.Net;
using System.Threading;
using System.Security.Cryptography;

namespace Cinema_Reservation
{
    public partial class screenedmovies : System.Web.UI.Page
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

            //ταινίες που παίζονται εντός 2 εβδομάδων από τη σημερινή
            Label2.Text = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.AddDays(14).ToShortDateString();

            try
            {
                var query = ((from m in context.MovieSet
                              join sc in context.ScreeningSet on m.Id equals sc.Movie.Id
                              where System.Data.Entity.DbFunctions.DiffDays(DateTime.Now, sc.Date) >= 0
                              select new { m.Id, m.Title, m.Year, m.Actor1, m.Actor2, m.AgeRating, m.ImageUrl })
                        .GroupBy(t => t.Title))
                        .Select(mv => mv.FirstOrDefault()); //επιλογή ταινιών που προβάλλονται από σήμερα και πέρα

                foreach (var item in query)
                {
                    HtmlGenericControl firstDiv = new HtmlGenericControl("div");
                    firstDiv.Attributes["class"] = "col-lg-4 mb-5";

                    HtmlGenericControl secDiv = new HtmlGenericControl("div");
                    secDiv.Attributes["class"] = "block-34";

                    HtmlGenericControl thirdDiv = new HtmlGenericControl("div");
                    thirdDiv.Attributes["class"] = "image";


                    Image image = new Image();
                    if (!string.IsNullOrWhiteSpace(item.ImageUrl)) //αν υπάρχει φωτογραφία για την ταινία
                    {
                        image.ImageUrl = "~/MovieImages//" + item.ImageUrl; //εύρεση φωτό από φάκελο project
                    }
                    else
                    {
                        image.ImageUrl = "~/MovieImages//filmReel.jpg"; //θα εμφανιστεί μια placeholder φωτό από φάκελο project
                    }
                    thirdDiv.Controls.Add(image);

                    secDiv.Controls.Add(thirdDiv);

                    HtmlGenericControl fourthDiv = new HtmlGenericControl("div");
                    fourthDiv.Attributes["class"] = "text";

                    Label label1 = new Label();
                    label1.Style.Add("class", "heading");
                    label1.Style.Add("font-size", "22px");
                    label1.Style.Add("font-weight", "bold");
                    label1.Text = item.Title; //ο τίτλος της ταινίας
                    fourthDiv.Controls.Add(label1);
                    fourthDiv.Controls.Add(new LiteralControl("<br/>"));

                    Label label4 = new Label();
                    label4.Style.Add("class", "heading");
                    label4.Style.Add("color", "grey");
                    label4.Text = item.Year; //έτος παραγωγής της ταινίας
                    fourthDiv.Controls.Add(label4);
                    fourthDiv.Controls.Add(new LiteralControl("<br/>"));

                    Label label2 = new Label();
                    label2.Style.Add("class", "heading");
                    label2.Style.Add("color", "grey");
                    label2.Text = item.Actor1; //ο 1ος ηθοποιός
                    fourthDiv.Controls.Add(label2);
                    fourthDiv.Controls.Add(new LiteralControl("<br/>"));


                    if (!string.IsNullOrWhiteSpace(item.Actor2)) //αν και δεύτερος ηθοποιός αποθηκευμένος σε ΒΔ
                    {
                        label2.Text += ","; //προσθέτουμε κόμμα σε text label2

                        Label label3 = new Label(); //πρόσθεση ονόματος και 2ου actor
                        label3.Style.Add("class", "heading");
                        label3.Style.Add("color", "grey");
                        label3.Text = item.Actor2; //ο 2ος ηθοποιός
                        fourthDiv.Controls.Add(label3);
                    }
                    fourthDiv.Controls.Add(new LiteralControl("<br/><br/>")); //2 breaks ακόμα, ανεξαρτήτως αν υπάρχει 2ος ηθοποιός


                    Button button = new Button();
                    button.ID = item.Id.ToString(); //ID για αναγνώριση συγκεκριμένης ταινίας που επέλεξε user
                    button.Text = "See more";
                    button.Style.Add("background-color", "#4D4D4D");
                    button.Style.Add("color", "white");
                    button.Style.Add("position", "relative");
                    button.Style.Add("cursor", "pointer");
                    button.Click += new EventHandler(SeeMoreButton_Click); //δίνουμε μια μέθοδο για το click event
                    fourthDiv.Controls.Add(button);



                    secDiv.Controls.Add(fourthDiv);
                    firstDiv.Controls.Add(secDiv);
                    rowdiv.Controls.Add(firstDiv);
                }
            }
            catch (Exception exc)
            {
                Label2.Text = "Something went wrong! Please try reloading the page";
            }
        }

        protected void SeeMoreButton_Click(object sender, EventArgs e) //κώδικας μεθόδου
        {
            Button pressedButton = (Button)sender; //κάνουμε cast το server object σε Button
            Session["chosenMovieId"] = pressedButton.ID; //αποθηκεύουμε την ID στο Session
            Response.Redirect("movieinfo.aspx"); //μετάβαση σε φόρμα με λεπτομέρειες επιλεγμένης ταινίας
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
                                        && c.RefreshToken.Equals(incomingRefresh.Substring(0,44))
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
                    else
                    {
                        HttpCookie refreshCookie = new HttpCookie("refreshCookie"); //διαγραφή refresh cookie από browser
                        refreshCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(refreshCookie);

                        Session["throwUnauthorizedExc"] = true; //τίθεται τιμή true στο αντίστοιχο key του Session
                        Response.Redirect("screenedmovies.aspx"); //refresh σελίδας, η PageLoad θα διαβάσει τιμή παραπάνω key και θα δώσει exception
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