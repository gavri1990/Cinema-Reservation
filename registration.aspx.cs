using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Security.Cryptography;
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace Cinema_Reservation
{
    public partial class registration : System.Web.UI.Page
    {
        Model1Container1 context = new Model1Container1(); //δημιουργία object της class ModelContainer για σύνδεση με ΒΔ
        AesClass aesMaster = new AesClass(); //αντικείμενο της class μας AesClass

        protected void Page_Load(object sender, EventArgs e)
        {
            Button2.Visible = false;
            Label3.Visible = false;
        }

        protected bool checkPassword(string password, string contentToCheckFor) //2 παράμετροι, password και τι θα ελεγχθεί, γυρνά τιμή boolean
        {
            if (contentToCheckFor.Equals("uppercaseCharacter")) //έλεγχος για κεφαλαίο χαρακτήρα
            {
                foreach (char c in password) //έλεγχος κάθε χαρακτήρα του password που έδωσε ο χρήστης
                {
                    if (char.IsUpper(c))
                    {
                        return true; //αν έστω ένας χαρακτήρας κεφαλαίο, γυρνά true
                    }
                }
                return false; //αν ελεγχθούν όλοι και δεν είναι κανένας κεφαλαίος, θα εκτελεστεί αυτή η γραμμή, γυρνλωντας false
            }
            else if (contentToCheckFor.Equals("lowercaseCharacter")) //έλεγχος για πεζό χαρακτήρα
            {
                foreach (char c in password)
                {
                    if (char.IsLower(c))
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (contentToCheckFor.Equals("digit"))//έλεγχος για αριθμό
            {
                foreach (char c in password)
                {
                    if (char.IsDigit(c))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                foreach (char c in password) //έλεγχος για σύμβολο
                {
                    if (!char.IsLetterOrDigit(c) && !char.IsControl(c))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        bool IsValidEmail(string email) //δέχεται ως παράμετρο ένα string για το email και ελέγχει αν έχει έγκυρη μορφή email, γυρνά τιμή boolean 
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email; //αν δεν υπάρξει κάποιο exception, θα γυρίσει true
            }
            catch
            {
                return false; //αν υπάρξει exception, θα γυρίσει false
            }
        }

        protected void Button1_Click(object sender, EventArgs e) //κουμπί Submit
        {
            if (string.IsNullOrWhiteSpace(TextBox1.Text) || string.IsNullOrWhiteSpace(TextBox2.Text)) //αν άδεια ή μόνο space τα πεδία
            {
                Label3.Visible = true;
                Label3.ForeColor = Color.Red;
                Label3.Text = "Please fill all required fields";
            }
            else if (TextBox2.Text.Trim().Length < 6) //trim ώστε να μην μετρηθούν οι χαρακτήρες space
            {
                Label3.Visible = true;
                Label3.ForeColor = Color.Red;
                Label3.Text = "Password must be at least 6 characters long";
            }
            else if (!checkPassword(TextBox2.Text, "uppercaseCharacter")) //αν η checkPassword για κεφαλαίο γυρίσει false
            {
                Label3.Visible = true;
                Label3.ForeColor = Color.Red;
                Label3.Text = "Password must contain at least 1 uppercase character";
            }
            else if (!checkPassword(TextBox2.Text, "lowercaseCharacter"))
            {
                Label3.Visible = true;
                Label3.ForeColor = Color.Red;
                Label3.Text = "Password must contain at least 1 lowercase character";
            }
            else if (!checkPassword(TextBox2.Text, "digit"))
            {
                Label3.Visible = true;
                Label3.ForeColor = Color.Red;
                Label3.Text = "Password must contain at least 1 digit";
            }
            else if (!checkPassword(TextBox2.Text, "symbol"))
            {
                Label3.Visible = true;
                Label3.ForeColor = Color.Red;
                Label3.Text = "Password must contain at least 1 symbol";
            }
            else if (!IsValidEmail(TextBox1.Text))
            {
                Label3.Visible = true;
                Label3.ForeColor = Color.Red;
                Label3.Text = "Enter a valid email address";
            }
            else
            {
                try
                {
                    byte[] salt;
                    var getSalt = new RNGCryptoServiceProvider();
                    getSalt.GetBytes(salt = new byte[32]); //καταχώριση τυχαίας αξίας 32 bytes στο salt
                    //κλήση Rfc2898DeriveBytes με τα παρακάτω ορίσματα για hashing του κωδικού χρήστη, αφού μπει το salt
                    var pbkdf2 = new Rfc2898DeriveBytes(TextBox2.Text, salt, 30000, new HashAlgorithmName("SHA256"));
                    byte[] hash = pbkdf2.GetBytes(32);
                    getSalt.Dispose(); //dispose των 2 αντικειμένων μετά τη χρησιμοποίησή τους
                    pbkdf2.Dispose();

                    Customer c = new Customer(); //δημιουργία νέου Customer
                    //encryption του email που δόθηκε από χρήστη σε array από bytes με χρήση μεθόδου class μας
                    byte[] encryptedEmail = aesMaster.EncryptStringToBytes_Aes(TextBox1.Text);
                    c.Email = Convert.ToBase64String(encryptedEmail); //μετατροπή σε base64 string και αποθήκευση στη ΒΔ
                    c.Password = Convert.ToBase64String(hash); //αποθήκευση hashed password και salt στη ΒΔ σε μορφή base 64 string
                    c.Salt = Convert.ToBase64String(salt);
                    context.CustomerSet.Add(c); //εισαγωγή στο table
                    context.SaveChanges(); //αποθήκευση αλλαγών σε ΒΔ

                    Label3.Visible = true;
                    Label3.ForeColor = Color.Green;
                    Label3.Text = "A verification link has been sent to your email account. " +
                        "Click on the link to verify your email and complete the registration process";
                    Button1.Visible = false;
                    Button2.Visible = true;


                    try
                    {
                        //δημιουργία email προς τον χρήστη που έδωσε ένα μη υπαρκτό στη ΒΔ email
                        using (MailMessage mail = new MailMessage())
                        {
                            mail.From = new MailAddress("cinemareservation2020@gmail.com");
                            mail.To.Add(TextBox1.Text);
                            mail.Subject = "Account Activation";
                            mail.Body = "Hello, " + Environment.NewLine + Environment.NewLine +
                                "We are excited to have you on CinemaReservation! " + Environment.NewLine +
                                 "To activate your account, please click on the link below." + Environment.NewLine +
                                 "-LINK-" + Environment.NewLine + Environment.NewLine + "Thank you!";

                            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                            {
                                //ανάκτηση encrypted στοιχείων email της εφαρμογής από το web.config
                                string emailAddr = ((NameValueCollection)WebConfigurationManager.GetSection("secureAppSettings"))["EmailAddress"];
                                string emailPass = ((NameValueCollection)WebConfigurationManager.GetSection("secureAppSettings"))["EmailPassword"];
                                smtp.Credentials = new NetworkCredential(emailAddr, emailPass);
                                smtp.EnableSsl = true; //ενεργοποίηση SSL
                                smtp.Send(mail); //αποστολή email
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Label3.Visible = true;
                        Label3.ForeColor = Color.Red;
                        Label3.Text = "Something went wrong! Please try reloading the page";
                    }
                }
                catch (Exception exc)
                {
                    //αν υπάρξει αυτό το exception, σημαίνει ότι υπήρξε violation στο unique πεδίο της ΒΔ, στο email
                    if (exc is System.Data.Entity.Infrastructure.DbUpdateException)
                    {
                        Label3.Visible = true;
                        Label3.ForeColor = Color.Green;
                        Label3.Text = "A verification link has been sent to your email account. Click on the link to verify your email and complete the registration process";
                        Button1.Visible = false;
                        Button2.Visible = true;

                        try
                        {
                            //δημιουργία email προς τον χρήστη του οποίου το email δόθηκε ενώ είναι ήδη υπαρκτό στη ΒΔ
                            using (MailMessage mail = new MailMessage())
                            {
                                mail.From = new MailAddress("cinemareservation2020@gmail.com");
                                mail.To.Add(TextBox1.Text);
                                mail.Subject = "Re-register Attempt";
                                mail.Body = "Hello, " + Environment.NewLine + Environment.NewLine +
                                    "Your email was used on an attempt to re-register at our service. " +
                                    "If you don't remember your password, go to the password reset page. " +
                                    "If you did not try to register with us again, please ignore this e-mail." +
                                     Environment.NewLine + Environment.NewLine + "Thank you!"; ;


                                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                                {
                                    //ανάκτηση encrypted στοιχείων email της εφαρμογής από το web.config
                                    string emailAddr = ((NameValueCollection)WebConfigurationManager.GetSection("secureAppSettings"))["EmailAddress"];
                                    string emailPass = ((NameValueCollection)WebConfigurationManager.GetSection("secureAppSettings"))["EmailPassword"];
                                    smtp.Credentials = new NetworkCredential(emailAddr, emailPass);
                                    smtp.EnableSsl = true; //ενεργοποίηση SSL
                                    smtp.Send(mail); //αποστολή email
                                }
                            }
                        }
                        catch (Exception exc1)
                        {
                            Label3.Visible = true;
                            Label3.ForeColor = Color.Red;
                            Label3.Text = "Something went wrong! Please try reloading the page";
                        }
                    }
                    else
                    {
                        Label3.Visible = true;
                        Label3.ForeColor = Color.Red;
                        Label3.Text = "Something went wrong! Please try reloading the page";
                    }
                }
            }
        }


        protected void Button2_Click(object sender, EventArgs e) //κουμπί Continue
        {
            Response.Redirect("login.aspx");
        }
    }
}