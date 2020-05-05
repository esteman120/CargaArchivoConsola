using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sincronizador
{
    public partial class Loguin : System.Windows.Forms.Form
    {
        public static ClientContext context;
        public static ClientContext contextHistorico;
        public static SecureString passWord;
        public Loguin()
        {
            InitializeComponent();
            btnIngresar.Enabled = true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            btnIngresar.Enabled = false;
            try
            {
                int contador = 0;
                if (txtRutaSitio.Text.Length == 0)
                {
                    MessageBox.Show("Por favor ingrese la ruta del sitio");
                    contador++;
                }
                else
                {
                    if (txtCorreo.Text.Length == 0)
                    {
                        MessageBox.Show("Por favor ingrese el correo");
                        contador++;
                    }
                    else
                    {
                        if (txtContrasena.Text.Length == 0)
                        {
                            MessageBox.Show("Por favor ingrese la contraseña");
                            contador++;
                        }
                    }
                }

                if (contador == 0)
                {
                    bool respuesta = ValidarCredenciales();
                    if (respuesta)
                    {
                        this.Hide();
                        Form1 sincronizador = new Form1(this, txtRutaSitio.Text, txtCorreo.Text, txtContrasena.Text);
                        sincronizador.Show();
                    }
                    else
                    {
                        MessageBox.Show("Error al ingresar al sitio por favor verifique el usuario y la contraseña");
                        btnIngresar.Enabled = true;
                    }
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Error al cargar la aplicación");
                btnIngresar.Enabled = true;
                //return false;
            }
        }        

        public static void LoguinGestionDocumental(string rutaSitio, string correo, string contrasena)
        {
            passWord = new SecureString();
            foreach (char c in contrasena.ToCharArray()) passWord.AppendChar(c);
            context = new ClientContext(rutaSitio);
            context.Credentials = new SharePointOnlineCredentials(correo, passWord);
        }

        public bool ValidarCredenciales()
        {
            try
            {
                LoguinGestionDocumental(txtRutaSitio.Text, txtCorreo.Text, txtContrasena.Text);
                Web web = context.Web;
                context.Load(web);
                context.ExecuteQuery();
                return true;
            }
            catch (Exception)
            {                
                return false;
            }
        }
    }
}
