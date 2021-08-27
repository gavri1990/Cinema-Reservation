using System;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Net;
using System.Web.Http;

namespace Cinema_Reservation
{
    public partial class login : System.Web.UI.Page
    {
        Model1Container1 context = new Model1Container1(); //δημιουργία object της class ModelContainer για σύνδεση με ΒΔ
        private JWTAuth jwtMaster = new JWTAuth(); //object της class μας JWTAuth
        AesClass aesMaster = new AesClass(); //object της class μας AesClass
        public const int mins = 5; //global σταθερές για ορισμό expiration των cookies 
        public const int days = 5;

        protected void Page_Load(object sender, EventArgs e)
        {          
            if (Session["throwUnauthorizedExc"] != null)
            {
                bool throwUnauthorizedExc = Convert.ToBoolean(Session["throwUnauthorizedExc"]);
                if (throwUnauthorizedExc == true) //αν το throwUnauthorizedExc είναι true
                {
                    throwUnauthorizedExc = false;
                    throw new HttpResponseException(HttpStatusCode.Unauthorized); //γίνεται throw το exception
                }
            }

            if (Request.Cookies["refreshCookie"] != null)
            {
                Session["remember"] = true; //αν υπάρχει refresh cookie σε browser client, είχε επιλέξει να τον θυμάται η σελίδα

                string incomingRefresh = Server.HtmlEncode(Request.Cookies["refreshCookie"].Value);

                //σύγκριση τελευταίου τμήματος refresh cookie με id και του προηγούμενου με αποθ. σε ΒΔ τμήμα refresh cookie
                var query = from c in context.CustomerSet
                            where c.Id.ToString().Equals(incomingRefresh.Substring(44)) 
                                    && c.RefreshToken.Equals(incomingRefresh.Substring(0,44))
                            select c;

                var query2 = from a in context.AdministratorSet
                             where a.Id.ToString().Equals(incomingRefresh.Substring(44)) 
                                    && a.RefreshToken.Equals(incomingRefresh.Substring(0,44))
                             select a;

                if (query.Any()) //αν το refresh token υπάρχει στην αντίστοιχη στήλη της καταχώρισης του χρήστη στη ΒΔ
                {
                    foreach (var cust in query) 
                    {
                        Session["customerId"] = cust.Id;

                        byte[] saltedCookie;
                        var getSalt = new RNGCryptoServiceProvider();
                        getSalt.GetBytes(saltedCookie = new byte[32]); //καταχώριση τυχαίας αξίας 32 bytes στο salt

                        string newSaltedCookie = Convert.ToBase64String(saltedCookie);
                        cust.RefreshToken = newSaltedCookie; //αποθήκευση τιμής νέου refreshtoken σε ΒΔ

                        //δημιουργία νέου refresh token, 'κολλάμε' στο τέλος το id του user
                        HttpCookie cookie = new HttpCookie("refreshCookie", newSaltedCookie + cust.Id.ToString());
                        cookie.HttpOnly = true; //ρυθμίσεις για περαιτέρω ασφάλεια cookie
                        cookie.Secure = true;
                        cookie.SameSite = SameSiteMode.Lax;
                        cookie.Expires = DateTime.Now.AddDays(days);
                        Response.Cookies.Add(cookie); //προσθήκη του στο Response του server προς τον browser
                    }
                    context.SaveChanges(); //αποθήκευση αλλαγών στη ΒΔ 

                    if (Request.Cookies["jwt"] != null) //αν υπάρχει και jwt cookie
                    {
                        string incomingToken = Server.HtmlEncode(Request.Cookies["jwt"].Value);

                        if (!jwtMaster.ValidateCurrentToken(incomingToken)) //validation του περιεχομένου jwt με χρήση μεθόδου της class μας
                        {
                            //αν ο έλεγχος είχε σφάλμα και γύρισε false
                            HttpCookie jwtCookie = new HttpCookie("jwt"); //διαγραφή cookie με το jwt από browser
                            jwtCookie.Expires = DateTime.Now.AddDays(-1d);
                            Response.Cookies.Add(jwtCookie);

                            HttpCookie refreshCookie = new HttpCookie("refreshCookie"); //διαγραφή refresh cookie από browser
                            refreshCookie.Expires = DateTime.Now.AddDays(-1d);
                            Response.Cookies.Add(refreshCookie);

                            Session["throwUnauthorizedExc"] = true; //τίθεται τιμή true στο αντίστοιχο key του Session
                            Response.Redirect("login.aspx"); //refresh σελίδας, η PageLoad θα διαβάσει τιμή παραπάνω key και θα δώσει exception
                        }
                        else
                        {
                            Response.Redirect("screenedmovies.aspx"); //αν το jwt είναι έγκυρο, μεταφερόμαστε σε αρχική σελίδα customer
                        }
                    }
                    else //αν δεν έχει jwt ο client  
                    {
                        int id = Convert.ToInt32(Session["customerId"]);

                        //βρίσκουμε τον customer σε ΒΔ μέσω Id
                        var query1 = from c in context.CustomerSet
                                     where c.Id.Equals(id)
                                     select c;

                        if (query1.Any())
                        {
                            foreach (var item1 in query1)
                            {
                                string token = jwtMaster.GenerateTokenUser(item1.Id); //δημιουργία jwt με χρήση μεθόδου class μας
                                HttpCookie cookie = new HttpCookie("jwt", token);
                                cookie.HttpOnly = true;
                                cookie.Secure = true;
                                cookie.SameSite = SameSiteMode.Lax;
                                cookie.Expires = DateTime.Now.AddMinutes(mins);
                                Response.Cookies.Add(cookie);

                                Response.Redirect("screenedmovies.aspx"); //μετάβαση στην αρχική σελίδα εφαρμογής χρήστη
                            }
                        }
                    }
                }              
                else if (query2.Any()) //αν το refresh token υπάρχει στην αντίστοιχη στήλη της καταχώρισης του admin στη ΒΔ
                {
                    foreach (var admin in query2)
                    {
                        Session["adminId"] = admin.Id;

                        byte[] saltedCookie;
                        var getSalt = new RNGCryptoServiceProvider();
                        getSalt.GetBytes(saltedCookie = new byte[32]); //καταχώριση τυχαίας αξίας 32 bytes στο salt

                        string newSaltedCookie = Convert.ToBase64String(saltedCookie);
                        admin.RefreshToken = newSaltedCookie; //αποθήκευση τιμής νέου refreshtoken σε ΒΔ

                        //δημιουργία νέου refresh token, 'κολλάμε' στο τέλος το id του admin
                        HttpCookie cookie = new HttpCookie("refreshCookie", newSaltedCookie + admin.Id.ToString());
                        cookie.HttpOnly = true; //ρυθμίσεις για περαιτέρω ασφάλεια cookie
                        cookie.Secure = true;
                        cookie.SameSite = SameSiteMode.Lax;
                        cookie.Expires = DateTime.Now.AddDays(days);
                        Response.Cookies.Add(cookie); //προσθήκη του στο Response του server προς τον browser
                    }
                    context.SaveChanges(); //αποθήκευση αλλαγών στη ΒΔ 

                    if (Request.Cookies["jwt"] != null) //αν υπάρχει και jwt cookie
                    {
                        string incomingToken = Server.HtmlEncode(Request.Cookies["jwt"].Value);

                        if (!jwtMaster.ValidateCurrentToken(incomingToken)) //validation του περιεχομένου jwt με χρήση μεθόδου της class μας
                        {
                            //αν ο έλεγχος είχε σφάλμα και γύρισε false
                            HttpCookie jwtCookie = new HttpCookie("jwt"); //διαγραφή cookie με το jwt από browser
                            jwtCookie.Expires = DateTime.Now.AddDays(-1d);
                            Response.Cookies.Add(jwtCookie);

                            HttpCookie refreshCookie = new HttpCookie("refreshCookie"); //διαγραφή refresh cookie από browser
                            refreshCookie.Expires = DateTime.Now.AddDays(-1d);
                            Response.Cookies.Add(refreshCookie);

                            Session["throwUnauthorizedExc"] = true; //τίθεται τιμή true στο αντίστοιχο key του Session
                            Response.Redirect("login.aspx"); //refresh σελίδας, η PageLoad θα διαβάσει τιμή παραπάνω key και θα δώσει exception
                        }
                        else
                        {
                            Response.Redirect("movies.aspx"); //αν το jwt είναι έγκυρο, μεταφερόμαστε σε αρχική σελίδα admin
                        }
                    }
                    else //αν δεν έχει jwt ο client 
                    {
                        int id1 = Convert.ToInt32(Session["adminId"]);

                        //βρίσκουμε τον customer σε ΒΔ μέσω Id
                        var query3 = from a in context.AdministratorSet
                                     where a.Id.Equals(id1)
                                     select a;

                        if (query3.Any())
                        {
                            foreach (var item3 in query3)
                            {
                                string token = jwtMaster.GenerateTokenAdmin(item3.Id); //δημιουργία jwt με χρήση μεθόδου class μας
                                HttpCookie cookie = new HttpCookie("jwt", token);
                                cookie.HttpOnly = true;
                                cookie.Secure = true;
                                cookie.SameSite = SameSiteMode.Lax;
                                cookie.Expires = DateTime.Now.AddMinutes(mins);
                                Response.Cookies.Add(cookie);

                                Response.Redirect("movies.aspx"); //μετάβαση στην αρχική σελίδα εφαρμογής admin
                            }
                        }
                    }
                }
                else //αν το refresh token του client δεν βρέθηκε στη ΒΔ, χρησιμοποιείται 2η φορά από κάποιον, κίνδυνος
                {
                    HttpCookie refreshCookie = new HttpCookie("refreshCookie"); //διαγραφή refresh cookie από browser
                    refreshCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(refreshCookie);

                    if (Request.Cookies["jwt"] != null) //αν υπάρχει και jwt
                    {
                        HttpCookie jwtCookie = new HttpCookie("jwt"); //διαγραφή cookie με το jwt από browser
                        jwtCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(jwtCookie);                    
                    }
                    Session["throwUnauthorizedExc"] = true; //τίθεται τιμή true στο αντίστοιχο key του Session
                    Response.Redirect("login.aspx"); //refresh σελίδας, η PageLoad θα διαβάσει τιμή παραπάνω key και θα δώσει exception                   
                }
            }
            Label1.Visible = false;           
        }

        protected void Button1_Click(object sender, EventArgs e) //κουμπί login
        {
            bool check = true;

            if (string.IsNullOrWhiteSpace(TextBox1.Text) || string.IsNullOrWhiteSpace(TextBox2.Text))
            {
                Label1.Text = "Please fill all required fields";
                Label1.Visible = true;
            }
            else
            {
                try
                {
                    //κάνουμε encrypt σε bytes το email που έδωσε ο χρήστης και το μετατρέπουμε σε string
                    byte[] encryptedInputEmailBytes = aesMaster.EncryptStringToBytes_Aes(TextBox1.Text);
                    string encryptedInputEmail = Convert.ToBase64String(encryptedInputEmailBytes);


                    //ψάχνουμε σε ΒΔ μέσω του κρυπτογραφημένου email
                    var query = from c in context.CustomerSet
                                where c.Email.Equals(encryptedInputEmail)
                                select new { c.Id, c.Email, c.Password, c.Salt };

                    var query2 = from a in context.AdministratorSet
                                 where a.Email.Equals(encryptedInputEmail)
                                 select new { a.Id, a.Email, a.Password, a.Salt };

                    if (query.Any()) //αν η 1η query δώσει αποτελέσματα, πρόκειται για guest
                    {
                        foreach (var item in query) //1 item, θα τρέξει μόνο μια φορά το loop
                        {
                            //ανάκτηση του hash και του salt από ΒΔ και μετατροπή τους σε bytes
                            byte[] savedHash = Convert.FromBase64String(item.Password);
                            byte[] savedSalt = Convert.FromBase64String(item.Salt);
                            //κλήση Rfc2898DeriveBytes για δημιουργία νέου hashed password από το input του χρήστη με προσθήκη salt 
                            var pbkdf2 = new Rfc2898DeriveBytes(TextBox2.Text, savedSalt, 30000, new HashAlgorithmName("SHA256"));
                            byte[] hash = pbkdf2.GetBytes(32);
                            pbkdf2.Dispose();//dispose του αντικειμένου μετά τη χρησιμοποίησή του

                            //σύγκριση hashed password που προέκυψε από την παραπάνω διαδικασία με ήδη αποθηκ. στη ΒΔ hashed password 
                            for (int i = 0; i < 32; i++) //σύγκριση κάθε byte των 2 hashed passwords
                            {
                                if (savedHash[i] != hash[i]) //αν κάποιο διαφέρει
                                {
                                    Label1.Text = "Incorrect email and/or password";
                                    Label1.Visible = true;
                                    check = false;
                                    break; //έξοδος από το loop
                                }
                            }

                            if (check.Equals(true))
                            {
                                if (CheckBox1.Checked.Equals(true))
                                {
                                    Session["customerId"] = item.Id; //περνάμε customer id στο Session
                                    Session["remember"] = true;

                                    //ψάχνουμε σε ΒΔ μέσω του κρυπτογραφημένου email
                                    var query3 = from c in context.CustomerSet
                                                 where c.Email.Equals(encryptedInputEmail)
                                                 select c;

                                    byte[] saltedCookie;
                                    var getSalt = new RNGCryptoServiceProvider();
                                    getSalt.GetBytes(saltedCookie = new byte[32]); //καταχώριση τυχαίας αξίας 32 bytes στο salt
                                    string tempId = null;

                                    foreach (var cust in query3)
                                    {
                                        tempId = cust.Id.ToString();
                                        cust.RefreshToken = Convert.ToBase64String(saltedCookie); //αποθήκευση τιμής νέου refreshtoken σε ΒΔ
                                    }

                                    string refreshToken = Convert.ToBase64String(saltedCookie);
                                    //δημιουργία νέου refresh token, 'κολλάμε' στο τέλος το id του user
                                    HttpCookie refreshCookie = new HttpCookie("refreshCookie", refreshToken + tempId);
                                    refreshCookie.HttpOnly = true; //ρυθμίσεις για περαιτέρω ασφάλεια cookie
                                    refreshCookie.Secure = true;
                                    refreshCookie.SameSite = SameSiteMode.Lax;
                                    refreshCookie.Expires = DateTime.Now.AddDays(days);
                                    Response.Cookies.Add(refreshCookie);

                                    string token = jwtMaster.GenerateTokenUser(item.Id); //δημιουργία jwt με χρήση μεθόδου class μας
                                    HttpCookie cookie = new HttpCookie("jwt", token);
                                    cookie.HttpOnly = true;
                                    cookie.Secure = true;
                                    cookie.SameSite = SameSiteMode.Lax;
                                    cookie.Expires = DateTime.Now.AddMinutes(mins);
                                    Response.Cookies.Add(cookie);
                                }
                                else //αν δεν επέλεξε να τον 'θυμάται' η εφαρμογή
                                {
                                    Session["customerId"] = item.Id; //περνάμε customer id στο Session
                                    Session["remember"] = false;

                                    //ψάχνουμε σε ΒΔ μέσω του κρυπτογραφημένου email
                                    var query3 = from c in context.CustomerSet
                                                 where c.Email.Equals(encryptedInputEmail)
                                                 select c;

                                    string token = jwtMaster.GenerateTokenUser(item.Id);  //δημιουργία (μόνο) jwt με χρήση μεθόδου class μας
                                    HttpCookie cookie = new HttpCookie("jwt", token);
                                    cookie.HttpOnly = true;
                                    cookie.Secure = true;
                                    cookie.SameSite = SameSiteMode.Lax;
                                    cookie.Expires = DateTime.Now.AddMinutes(mins);
                                    Response.Cookies.Add(cookie);
                                }                               
                            }
                        }
                        context.SaveChanges(); //αποθήκευση αλλαγών σε ΒΔ

                        if (check.Equals(true))
                            Response.Redirect("screenedmovies.aspx");
                    }
                    else if (query2.Any()) //αν η 1η query δώσει αποτελέσματα, πρόκειται για τον admin
                    {
                        foreach (var item2 in query2) //1 item, θα τρέξει μόνο μια φορά το loop
                        {
                            //ανάκτηση του hash και του salt από ΒΔ και μετατροπή τους σε bytes
                            byte[] savedHash = Convert.FromBase64String(item2.Password);
                            byte[] savedSalt = Convert.FromBase64String(item2.Salt);
                            //κλήση Rfc2898DeriveBytes για δημιουργία νέου hashed password από το input του χρήστη με προσθήκη salt 
                            var pbkdf2 = new Rfc2898DeriveBytes(TextBox2.Text, savedSalt, 30000, new HashAlgorithmName("SHA256"));
                            byte[] hash = pbkdf2.GetBytes(32);
                            pbkdf2.Dispose(); //dispose του αντικειμένου μετά τη χρησιμοποίησή του

                            //σύγκριση hashed password που προέκυψε από την παραπάνω διαδικασία με ήδη αποθηκ. στη ΒΔ hashed password
                            for (int i = 0; i < 32; i++) //σύγκριση κάθε byte των 2 hashed passwords
                            {
                                if (savedHash[i] != hash[i]) //αν κάποιο διαφέρει
                                {
                                    Label1.Text = "Incorrect email and/or password";
                                    Label1.Visible = true;
                                    check = false;
                                    break; //έξοδος από το loop
                                }
                            }

                            if (check.Equals(true))
                            {
                                if (CheckBox1.Checked.Equals(true))
                                {
                                    Session["adminId"] = item2.Id; //περνάμε admin id στο Session
                                    Session["remember"] = true;

                                    //ψάχνουμε σε ΒΔ μέσω του κρυπτογραφημένου email
                                    var query4 = from a in context.AdministratorSet
                                                 where a.Email.Equals(encryptedInputEmail)
                                                 select a;

                                    byte[] saltedCookie;
                                    var getSalt = new RNGCryptoServiceProvider();
                                    getSalt.GetBytes(saltedCookie = new byte[32]); //καταχώριση τυχαίας αξίας 32 bytes στο salt
                                    string tempId = null;

                                    foreach (var admin in query4)
                                    {
                                        tempId = admin.Id.ToString();
                                        admin.RefreshToken = Convert.ToBase64String(saltedCookie); //αποθήκευση τιμής νέου refreshtoken σε ΒΔ
                                    }

                                    string refreshToken = Convert.ToBase64String(saltedCookie);
                                    //δημιουργία νέου refresh token
                                    HttpCookie refreshCookie = new HttpCookie("refreshCookie", refreshToken + tempId);
                                    refreshCookie.HttpOnly = true; //ρυθμίσεις για περαιτέρω ασφάλεια cookie
                                    refreshCookie.Secure = true;
                                    refreshCookie.SameSite = SameSiteMode.Lax;
                                    refreshCookie.Expires = DateTime.Now.AddDays(days);
                                    Response.Cookies.Add(refreshCookie);

                                    string token = jwtMaster.GenerateTokenAdmin(item2.Id); //δημιουργία jwt με χρήση μεθόδου class μας
                                    HttpCookie cookie = new HttpCookie("jwt", token);
                                    cookie.HttpOnly = true;
                                    cookie.Secure = true;
                                    cookie.SameSite = SameSiteMode.Lax;
                                    cookie.Expires = DateTime.Now.AddMinutes(mins);
                                    Response.Cookies.Add(cookie);
                                }
                                else //αν δεν επέλεξε να τον 'θυμάται' η εφαρμογή
                                {
                                    Session["adminId"] = item2.Id; //περνάμε admin id στο Session
                                    Session["remember"] = false;

                                    //ψάχνουμε σε ΒΔ μέσω του κρυπτογραφημένου email
                                    var query4 = from a in context.AdministratorSet
                                                 where a.Email.Equals(encryptedInputEmail)
                                                 select a;

                                    string token = jwtMaster.GenerateTokenAdmin(item2.Id); //δημιουργία jwt με χρήση μεθόδου class μας
                                    HttpCookie cookie = new HttpCookie("jwt", token);
                                    cookie.HttpOnly = true;
                                    cookie.Secure = true;
                                    cookie.SameSite = SameSiteMode.Lax;
                                    cookie.Expires = DateTime.Now.AddMinutes(mins);
                                    Response.Cookies.Add(cookie);
                                }
                            }
                        }
                        context.SaveChanges(); //αποθήκευση αλλαγών σε ΒΔ

                        if (check.Equals(true))
                            Response.Redirect("movies.aspx");
                    }
                    else //αν δόθηκε λάθος email ή/και password
                    {
                        Label1.Text = "Incorrect email and/or password";
                        Label1.Visible = true;
                    }
                }
                catch (Exception exc) //σε περίπτωση exception
                {
                    Label1.Visible = true;
                    Label1.Text = "Something went wrong! Please try reloading the page";
                }
            }
        }
    }
}