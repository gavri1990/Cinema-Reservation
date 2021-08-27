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
    public partial class movies : System.Web.UI.Page
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

            if (!IsPostBack) //πρώτη φορά στο page, όχι μέσω refresh
            {
                Label3.Visible = false;
                Button2.Visible = false;
                Button6.Visible = false;
                Button7.Visible = false;
            }           
        }

        protected void Button1_Click(object sender, EventArgs e) //κουμπί Add Movie
        {
            //απαραίτητοι έλεγχοι input χρήστη
            if (string.IsNullOrWhiteSpace(TextBox1.Text) || string.IsNullOrWhiteSpace(TextBox2.Text) || string.IsNullOrWhiteSpace(TextBox3.Text) ||
                string.IsNullOrWhiteSpace(TextBox4.Text) || string.IsNullOrWhiteSpace(TextBox5.Text) || DropDownList1.SelectedValue.Equals("Age Rating"))
            {
                Label3.Visible = true;
                Label3.ForeColor = Color.Red;
                Label3.Text = "Please fill all required fields";
                HyperLink1.Focus();
            }
            else if (TextBox3.Text.Length < 4)
            {
                Label3.Visible = true;
                Label3.ForeColor = Color.Red;
                Label3.Text = "'Year' field requires 4 digits";
                HyperLink1.Focus();
            }
            else
            {
                try
                {
                    Movie movie = new Movie(); //δημιουργία νέας καταχώρισης Movie
                    movie.Title = TextBox1.Text;
                    movie.Duration = TextBox2.Text;
                    movie.Year = TextBox3.Text;
                    movie.AgeRating = DropDownList1.SelectedValue;
                    movie.Director = TextBox4.Text;
                    movie.Actor1 = TextBox5.Text;
                    movie.Actor2 = TextBox6.Text;
                    movie.Actor3 = TextBox7.Text;
                    if (FileUpload1.HasFile)
                    {
                        movie.ImageUrl = FileUpload1.FileName; //αποθήκευση ονόματος αρχείου σε ΒΔ
                        FileUpload1.SaveAs(Server.MapPath("~/MovieImages//" + FileUpload1.FileName)); //αποθήκευση και στον φάκελο του project      
                    }

                    context.MovieSet.Add(movie); //προσθήκη σε πίνακα ΒΔ
                    context.SaveChanges(); //αποθήκευση αλλαγών σε ΒΔ

                    Label3.Visible = true;
                    Label3.ForeColor = Color.Green;
                    Label3.Text = "Movie entry added successfully! Do you want to add another one?";

                    TextBox1.Enabled = false; //απενεργοποίηση textboxes κ.α.
                    TextBox2.Enabled = false;
                    TextBox3.Enabled = false;
                    TextBox4.Enabled = false;
                    TextBox5.Enabled = false;
                    TextBox6.Enabled = false;
                    TextBox7.Enabled = false;
                    labelgray.Visible = false;
                    HyperLink2.Visible = false;
                    DropDownList1.Enabled = false;
                    DropDownList1.Style.Add("cursor", "default");
                    FileUpload1.Enabled = false;

                    Button1.Visible = false;
                    Button6.Visible = true;
                    Button7.Visible = true;
                    Button6.Focus();
                }
                catch(Exception exc)
                {

                    Label3.Visible = true; //εμφάνιση μηνύματος
                    Label3.Text = "Entry already exists or something else went wrong. Please try again";
                    Label3.ForeColor = Color.Red;
                }
            }
        }

        protected void Button2_Click(object sender, EventArgs e) //κουμπί Continue
        {
            Response.Redirect("schedule.aspx");
        }

        protected void Button6_Click(object sender, EventArgs e)//κουμπί Yes
        {
            TextBox1.Enabled = true; //επανενεργοποίηση textboxes κ.α.
            TextBox2.Enabled = true;
            TextBox3.Enabled = true;
            TextBox4.Enabled = true;
            TextBox5.Enabled = true;
            TextBox6.Enabled = true;
            TextBox7.Enabled = true;
            labelgray.Visible = true;
            HyperLink2.Visible = true;
            DropDownList1.Enabled = true;
            DropDownList1.Style.Add("cursor", "pointer");
            FileUpload1.Enabled = true;

            TextBox1.Text = ""; //καθαρισμός TextBoxes
            TextBox2.Text = "";
            TextBox3.Text = "";
            DropDownList1.SelectedIndex = 0; //επαναφορά σε 1ο στοιχείο dropdownlist
            TextBox4.Text = "";
            TextBox5.Text = "";
            TextBox6.Text = "";
            TextBox7.Text = "";
            Button1.Visible = true;
            Button6.Visible = false;
            Button7.Visible = false;
            Label3.Visible = false;
        }

        protected void Button7_Click(object sender, EventArgs e) //κουμπί No
        {
            Button6.Visible = false;
            Button7.Visible = false;
            Button2.Visible = true;
            Label3.Visible = false;
            Button2.Focus();
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
                    else //αν δεν υπάρχει σε στήλη της ΒΔ το refresh, χρησιμοποιείται 2η φορά από κάποιον
                    {
                        HttpCookie refreshCookie = new HttpCookie("refreshCookie"); //διαγραφή refresh cookie από browser
                        refreshCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(refreshCookie);

                        Session["throwUnauthorizedExc"] = true; //τίθεται τιμή true στο αντίστοιχο key του Session
                        Response.Redirect("movies.aspx"); //refresh σελίδας, η PageLoad θα διαβάσει τιμή παραπάνω key και θα δώσει exception
                    }
                }
                else //αν δεν έχει επιλέξει να τον 'θυμάται' η εφαρμογή, δημιουργείται μόνο jwt
                {
                    int id = Convert.ToInt32(Session["adminId"]);
                    string token = jwtMaster.GenerateTokenAdmin(id);
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