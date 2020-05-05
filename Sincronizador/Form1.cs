using ExcelDataReader;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sincronizador
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public static ClientContext context;
        public static ClientContext contextHistorico;
        public static SecureString passWord;
        string rutaArchivo = "";
        Loguin Padre = null;
        string ruta = "";
        string Correo = "";
        string Contrasena = "";
        string nombreDocLog = "";
        public Form1(Loguin prmPadre, string rutaSitio,string  correo, string contrasena)
        {
            InitializeComponent();
            Padre = prmPadre;
            ruta = rutaSitio;
            Correo = correo;
            Contrasena = contrasena;
        }

        public static void LoguinGestionDocumental(string rutaSitio, string usuario ,string contrasena)
        {
            passWord = new SecureString();
            foreach (char c in contrasena.ToCharArray()) passWord.AppendChar(c);
            context = new ClientContext(rutaSitio);
            context.Credentials = new SharePointOnlineCredentials(usuario, passWord);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Office Files|*.xls;*.xlsx";
            if (open.ShowDialog() == DialogResult.OK)
            {
                rutaArchivo = open.FileName;
                try
                {
                    FileStream fs = System.IO.File.Open(rutaArchivo, FileMode.Open, FileAccess.Read);
                    label1.Text = rutaArchivo;
                    fs.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);                    
                }
            }
            else if (open.ShowDialog() == DialogResult.Cancel)
            {
                label1.Text = "";
            }
            
            open.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        { 
            ejecucionArchivos();
                     
        }

        public void ejecucionArchivos()
        {
            button2.Enabled = false;

            DateTime fecha = DateTime.Now;

            string dia = fecha.Day.ToString().Length == 1 ? "0" + fecha.Day.ToString() : fecha.Day.ToString();
            string mes = (fecha.Month + 1).ToString().Length == 1 ? "0" + (fecha.Month + 1).ToString() : (fecha.Month + 1).ToString();
            string year = fecha.Year.ToString();
            string hora = fecha.Hour.ToString().Length == 1 ? "0" + fecha.Hour.ToString() : fecha.Hour.ToString();
            string minuto = fecha.Minute.ToString().Length == 1 ? "0" + fecha.Minute.ToString() : fecha.Minute.ToString();
            string segundo = fecha.Second.ToString().Length == 1 ? "0" + fecha.Second.ToString() : fecha.Second.ToString();

            nombreDocLog = "Log" + dia + mes + year + hora + minuto + segundo;

            LoguinGestionDocumental(ruta, Correo, Contrasena);
            DataTable dtANC = new DataTable("ArchivosNoCargados");
            dtANC.Columns.Add("nombreArchivo");
            dtANC.Columns.Add("NombreCliente");
            dtANC.Columns.Add("CodJob");
            dtANC.Columns.Add("UnidadNegocio");
            dtANC.Columns.Add("Encargado");
            dtANC.Columns.Add("FechaInicio");
            dtANC.Columns.Add("FechaFin");
            dtANC.Columns.Add("TituloDescripcion");
            dtANC.Columns.Add("UbicacionAntigua");

            DataTable dtAC = new DataTable("ArchivosCargados");
            dtAC.Columns.Add("nombreArchivo");
            dtAC.Columns.Add("NombreCliente");
            dtAC.Columns.Add("CodJob");
            dtAC.Columns.Add("UnidadNegocio");
            dtAC.Columns.Add("Encargado");
            dtAC.Columns.Add("FechaInicio");
            dtAC.Columns.Add("FechaFin");
            dtAC.Columns.Add("TituloDescripcion");
            dtAC.Columns.Add("UbicacionAntigua");
            try
            {
                
                FileStream fs = System.IO.File.Open(rutaArchivo, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                DataSet result = reader.AsDataSet();
                DataTable table = result.Tables["Prueba BD General AH"];

                if (Directory.Exists(txtRutaCarpeta.Text))
                {
                    DirectoryInfo di = new DirectoryInfo(txtRutaCarpeta.Text);

                    for (int i = 1; i < table.Rows.Count - 1; i++)
                    {
                        var Columna1 = table.Rows[i]["Column0"].ToString();
                        var Columna2 = table.Rows[i]["Column1"].ToString();
                        var Columna3 = table.Rows[i]["Column2"].ToString();
                        var Columna4 = table.Rows[i]["Column3"].ToString();
                        var Columna5 = table.Rows[i]["Column4"].ToString();
                        var Columna6 = table.Rows[i]["Column5"].ToString();
                        var Columna7 = table.Rows[i]["Column6"].ToString();
                        var Columna8 = table.Rows[i]["Column7"].ToString();
                        var Columna9 = table.Rows[i]["Column8"].ToString();


                        var fileEntries = di.GetFiles(Columna1 + ".pdf");
                        var contador = fileEntries.Count();
                        if (contador > 0)
                        {
                            var ppp = fileEntries[0];
                            var path = ppp.FullName;
                            var respuestaCarga = CargarDocumentos(
                                path,
                                txtNombreBiblioteca.Text,
                                Columna1,
                                Columna2,
                                Columna3,
                                Columna4,
                                Columna5,
                                Columna6,
                                Columna7,
                                Columna8,
                                Columna9);

                            if (respuestaCarga)
                            {
                                DataRow row = dtAC.NewRow();
                                row["nombreArchivo"] = Columna1;
                                row["NombreCliente"] = Columna2;
                                row["CodJob"] = Columna3;
                                row["UnidadNegocio"] = Columna4;
                                row["Encargado"] = Columna5;
                                row["FechaInicio"] = Columna6;
                                row["FechaFin"] = Columna7;
                                row["TituloDescripcion"] = Columna8;
                                row["UbicacionAntigua"] = Columna9;
                                dtAC.Rows.Add(row);
                                dgvArchivosCargados.DataSource = dtAC;
                            }
                            else
                            {
                                DataRow row = dtANC.NewRow();
                                row["nombreArchivo"] = Columna1;
                                row["NombreCliente"] = Columna2;
                                row["CodJob"] = Columna3;
                                row["UnidadNegocio"] = Columna4;
                                row["Encargado"] = Columna5;
                                row["FechaInicio"] = Columna6;
                                row["FechaFin"] = Columna7;
                                row["TituloDescripcion"] = Columna8;
                                row["UbicacionAntigua"] = Columna9;
                                dtANC.Rows.Add(row);
                                dgvArchivosNoCargados.DataSource = dtANC;
                            }
                        }
                    }
                    fs.Close();
                    button2.Enabled = true;
                }
                else
                {
                    MessageBox.Show("La ruta de la carpeta no ha sido encontrada, por favor verifiquela");
                    button2.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                button2.Enabled = true;
                MessageBox.Show(ex.Message);
                
            }
        }

        public Boolean CargarDocumentos(string path, 
            string nombreBiblioteca, 
            string nombreDoc, 
            string nombreCliente,
            string codJob,
            string unidadNegocio,
            string encargado,
            string fechaIni,
            string fechaFin,
            string tituloDescripcion,
            string ubicacionAntigua)
        {
            Boolean respuesta = false;
            try
            {
                Web web = context.Web;
                FileCreationInformation newFile = new FileCreationInformation();
                byte[] FileContent = System.IO.File.ReadAllBytes(path);
                newFile.ContentStream = new MemoryStream(FileContent);

                newFile.Url = Path.GetFileName(path);
                List docs = web.Lists.GetByTitle(nombreBiblioteca);
                Microsoft.SharePoint.Client.File uploadFile = docs.RootFolder.Files.Add(newFile);
                uploadFile.ListItemAllFields["Title"] = nombreDoc;
                uploadFile.ListItemAllFields["NombreCliente"] = nombreCliente;
                uploadFile.ListItemAllFields["CodJob"] = codJob;
                uploadFile.ListItemAllFields["UnidadNegocio"] = unidadNegocio;
                uploadFile.ListItemAllFields["Encargado"] = encargado;
                uploadFile.ListItemAllFields["FechaInicio"] = fechaIni;
                uploadFile.ListItemAllFields["FechaFin"] = fechaFin;
                uploadFile.ListItemAllFields["TituloDescripcion"] = tituloDescripcion;
                uploadFile.ListItemAllFields["UbicacionAntigua"] = ubicacionAntigua;
                uploadFile.ListItemAllFields.Update();
                context.Load(docs);
                context.Load(uploadFile);
                context.ExecuteQuery();
                respuesta = true;
                return respuesta;
            }
            catch (Exception ex)
            {
                string filePath = @txtRutaCarpeta.Text+"/"+ nombreDocLog + ".txt";
                if (!System.IO.File.Exists(filePath)) // If file does not exists
                {
                    System.IO.File.Create(filePath).Close(); // Create file
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("-----------------------------------------------------------------------------");
                        writer.WriteLine("Date : " + DateTime.Now.ToString());
                        writer.WriteLine();

                        while (ex != null)
                        {
                            writer.WriteLine(ex.GetType().FullName);
                            writer.WriteLine("Message : " + ex.Message);
                            //writer.WriteLine("StackTrace : " + ex.StackTrace);
                            writer.WriteLine("Error al guardar los documentos");

                            ex = ex.InnerException;
                        }
                    }
                }
                else // If file already exists
                {
                    // File.WriteAllText("FILENAME.txt", String.Empty); // Clear file
                   
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine("-----------------------------------------------------------------------------");
                        writer.WriteLine("Date : " + DateTime.Now.ToString());
                        writer.WriteLine();

                        while (ex != null)
                        {
                            writer.WriteLine(ex.GetType().FullName);
                            writer.WriteLine("Message : " + ex.Message);
                            //writer.WriteLine("StackTrace : " + ex.StackTrace);
                            writer.WriteLine("Error al guardar los documentos");

                            ex = ex.InnerException;
                        }
                    }
                    
                }                


                respuesta = false;
                return respuesta;
            }            
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Padre.Close();
        }
    }
}
