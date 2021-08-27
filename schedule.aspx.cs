using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Security.Cryptography;
using System.Web.Http;
using System.Net;

namespace Cinema_Reservation
{
    public partial class admin : System.Web.UI.Page
    {
        Model1Container1 context = new Model1Container1(); //δημιουργία object της class ModelContainer για σύνδεση με ΒΔ
        private JWTAuth jwtMaster = new JWTAuth();
        static int duration; //θα κρατά τη διάρκεια της επιλεγμένης ταινίας
        int daysToAdd; //για δημιουργία τόσων προβολών όσες και οι μέρες που επιλέχθηκαν για προβολή ταινίας

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

            Label3.Visible = false;

            if (!IsPostBack) //αν ερχόμαστε πρώτη φορά στο page, όχι μέσω refresh
            {
                var query = from m in context.MovieSet
                            select new {m.Id, m.Title};

                foreach (var item in query)
                { 
                    DropDownList1.Items.Add(new ListItem(item.Title, item.Id.ToString())); //περνάμε το id ως value σε κάθε item της dropdownlist
                }
            }           
        }

        protected void Button1_Click(object sender, EventArgs e) //κουμπί Set Schedule
        {
            //αν δεν έχει γίνει έγκυρη επιλογή σε κάποια dropdownlist
            if (DropDownList1.SelectedValue.Equals("Title") || DropDownList2.SelectedValue.Equals("Screen") ||
                DropDownList3.SelectedValue.Equals("Start Time") || DropDownList4.SelectedValue.Equals("Screening Period"))
            {
                Label3.Visible = true;
                Label3.ForeColor = Color.Red;
                Label3.Text = "Please make all required selections";
            }
            else
            {             
                DateTime today = DateTime.Today;
                DateTime lastDay;

                if (DropDownList4.SelectedValue.Substring(2).Equals("Day") 
                    || DropDownList4.SelectedValue.Substring(2).Equals("Days")) //έλεγχος διάρκειας προβολής screening σε ημέρες
                {
                    //υπολογισμός διάρκειας προβολής ταινίας σε ημέρες
                    daysToAdd = Convert.ToInt32(DropDownList4.SelectedValue.Substring(0, 1));
                    lastDay = today.AddDays(daysToAdd); //προσθήκη στη σημερινή ημερομηνία του αριθμού των ημερών που επέλεξε ο διαχειριστής
                }
                else if (DropDownList4.SelectedValue.Substring(2).Equals("Week")
                    || DropDownList4.SelectedValue.Substring(2).Equals("Weeks")) //έλεγχος διάρκειας προβολής screening σε εβδομάδες
                {
                    int weeks = Convert.ToInt32(DropDownList4.SelectedValue.Substring(0, 1));
                    daysToAdd = weeks * 7; //7 μέρες ανά εβδομάδα
                    lastDay = today.AddDays(daysToAdd);
                }

                //αποθηκεύουμε σε μεταβλητές τα movie και screen ids που υπάρχουν ως values στα DropDownLists
                int selectedMovieId = Convert.ToInt32(DropDownList1.SelectedItem.Value);
                int selectedScreenId = Convert.ToInt32(DropDownList2.SelectedItem.Value);


                //κάθε ένα από τα παρακάτω queries θα έχει 1 αποτέλεσμα
                var query1 = from movie in context.MovieSet
                             where movie.Id == selectedMovieId
                             select movie;

                var query2 = from screen in context.ScreenSet
                             where screen.Id == selectedScreenId
                             select screen;

                try
                {
                    for (int days = 0; days < daysToAdd; days++) //για όσες μέρες επιλέχθηκε να προβληθεί η ταινία, ξεκινώντας από σήμερα
                    {
                        //κατασκευή μιας εγγραφής Screening
                        Screening screening = new Screening();
                        screening.Date = today.AddDays(days);
                        screening.StartTime = DateTime.Parse(DropDownList3.SelectedValue,
                                                System.Globalization.CultureInfo.InvariantCulture);
                        screening.EndTime = screening.StartTime.Add(new TimeSpan(0, duration, 0));

                        foreach(var movie in query1)
                        {
                            movie.Screening.Add(screening); //σύνδεση κάθε screening που δημιουργείται με την επιλεγμένη ταινία
                        }

                        foreach (var screen in query2)
                        {
                            screen.Screening.Add(screening); //σύνδεση κάθε screening που δημιουργείται με την επιλεγμένη αίθουσα
                        }
                    }
                    context.SaveChanges(); //αποθήκευση αλλαγών σε ΒΔ

                    Label3.Visible = true;
                    Label3.ForeColor = Color.Green;
                    Label3.Text = "Movie schedule set successfully!";

                    DropDownList1.SelectedIndex = 0; //επαναφορά σε 1ο στοιχείο DropDownlists
                    DropDownList2.SelectedIndex = 0;
                    DropDownList3.SelectedIndex = 0;
                    DropDownList4.SelectedIndex = 0;

                }
                catch(Exception exc)
                {
                    Label3.Visible = true; //εμφάνιση μηνύματος
                    Label3.Text = "Screening already scheduled or something else went wrong. Please try again";
                    Label3.ForeColor = Color.Red;
                }
            }
        }


        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e) //μόλις επιλεχθεί μια ταινία στη DropDownList
        {
            int selectedMovieId = Convert.ToInt32(DropDownList1.SelectedItem.Value);

            //βρίσκουμε διάρκεια της ταινίας ψάχνοντάς την μέσω id
            var query = from m in context.MovieSet
                        where m.Id == selectedMovieId
                        select new { m.Duration };

            foreach (var item in query)
            {
                duration = Convert.ToInt32(item.Duration); //ανάθεση διάρκειας σε static μεταβλητή
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
                    var query = from a in context.AdministratorSet
                                where a.Id.ToString().Equals(incomingRefresh.Substring(44))
                                       && a.RefreshToken.Equals(incomingRefresh.Substring(0, 44))
                                select a;

                    if (query.Any()) //αν υπάρχει σε στήλη της ΒΔ το refresh, δεν χρησιμοποιείται 2η φορά από κάποιον
                    {
                        foreach (var admin in query)
                        {
                            byte[] saltedCookie;
                            var getSalt = new RNGCryptoServiceProvider();
                            getSalt.GetBytes(saltedCookie = new byte[32]); //καταχώριση τυχαίας αξίας 32 bytes στο salt

                            string newSaltedCookie = Convert.ToBase64String(saltedCookie);
                            admin.RefreshToken = newSaltedCookie; //αποθήκευση τιμής νέου refreshtoken σε ΒΔ

                            //νέο refresh cookie
                            HttpCookie refreshCookie = new HttpCookie("refreshCookie", newSaltedCookie + admin.Id.ToString());
                            refreshCookie.HttpOnly = true; //ρυθμίσεις για περαιτέρω ασφάλεια cookie
                            refreshCookie.Secure = true;
                            refreshCookie.SameSite = SameSiteMode.Lax;
                            refreshCookie.Expires = DateTime.Now.AddDays(5);
                            Response.Cookies.Add(refreshCookie); //προσθήκη του στο Response του server προς τον browser

                            //νέο jwt token σε cookie
                            int id = Convert.ToInt32(Session["adminId"]);
                            string token = jwtMaster.GenerateTokenAdmin(id);
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
                        Response.Redirect("schedule.aspx"); //refresh σελίδας, η PageLoad θα διαβάσει τιμή παραπάνω key και θα δώσει exception
                    }
                }
                else //αν δεν έχει επιλέξει να τον 'θυμάται' η εφαρμογή, δημιουργείται μόνο jwt
                {
                    int id = Convert.ToInt32(Session["adminId"]);
                    string token = jwtMaster.GenerateTokenAdmin(id); //δημιουργία jwt με χρήση μεθόδου class μας
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
            var query2 = from a in context.AdministratorSet
                         where a.Id.ToString().Equals(incomingRefresh.Substring(44))
                                && a.RefreshToken.Equals(incomingRefresh.Substring(0, 44))
                         select a;

            if (query2.Any())
            {
                foreach (var admin in query2)
                {
                    admin.RefreshToken = null; //null σε πεδίο ΒΔ με refresh token, ώστε να μην γίνει δεκτό αν χρησιμ. 2η φορά από επιτεθέμενο
                }
            }
            context.SaveChanges(); //αποθήκευση αλλαγών στη ΒΔ
            Response.Redirect("login.aspx"); //επιστροφή στη login
        }
    }
}