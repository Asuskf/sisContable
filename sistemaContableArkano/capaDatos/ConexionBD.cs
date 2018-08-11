﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace capaDatos
{
    public class ConexionBD
    {

        //cadena de conexion
        //private string cadenaBD = "Data Source=DESKTOP-A4GPQL3;Initial Catalog=SistemaContableArcano2;Integrated Security=True";
        //private string cadenaBD = "Data Source=SMLAB304PC21_17;Initial Catalog=SistemaContableArcano2;User ID=SA;Password=Uisrael2018"; 
        private string cadenaBD = "Data Source=DESKTOP-TL8NQ49\\ALEXGD;Initial Catalog=SistemaContableArcano2; Persist Security Info= True; Integrated Security=True";
        public SqlConnection cnn;
        private SqlCommandBuilder cmb;
        public DataSet ds = new DataSet();  //el data set guarda varias tablas llamadas Datatable que nos servira para mostrar los datos
        public SqlDataAdapter adapt;
        public DataRow dr;
        public SqlCommand comando;
        public DataTable dt1;
        private Image nothing;

        public void conectar()
        {
            cnn = new SqlConnection(cadenaBD);
        }

        public ConexionBD()
        {
            conectar();

        }

        //////Metodo para consultar sentencias "select"  
        public void consultar(string sql, string tabla)
        {
            try
            {
                ds.Tables.Clear();
                adapt = new SqlDataAdapter(sql, cnn);
                cmb = new SqlCommandBuilder(adapt);
                adapt.Fill(ds, tabla);
            }
            catch (Exception ex)
            {
            }
        }


        /////////Metodo para eliminar
        public bool eliminar(string setencia2)
        {
            cnn.Open();
            comando = new SqlCommand(setencia2, cnn);
            int i = comando.ExecuteNonQuery();
            cnn.Close();
            if (1 > 0)
                return true;
            else
                return false;

        }


        /////////Metodo para actualizar
        public bool actualizar(string sentencia)
        {
            cnn.Open();
            comando = new SqlCommand(sentencia, cnn);
            int i = comando.ExecuteNonQuery();
            cnn.Close();

            if (i > 0)
                return true;
            else
                return false;
        }


        ///////////Metodo para insertar
        public bool insertar(string sql4)
        {
            cnn.Open();
            comando = new SqlCommand(sql4, cnn);
            int i = comando.ExecuteNonQuery();
            cnn.Close();
            if (i > 0)
                return true;
            else
                return false;

        }

        public string insertarUsuario(string usuNombre, string usuApellido, string usuDireccion, string usuCorreo, string usuNombreUsuario, string usuContrasena) {
            string mensaje = "Usuario creado con exito";
            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            try
            {
                comando = new SqlCommand("insert into TB_USUARIO (usuNombre,usuApellido,usuDireccion,usuCorreo,usuNombreUsuario,usuContrasena) values ('" + usuNombre + "', '" + usuApellido + "', '" + usuDireccion + "', " +
                    "'" + usuCorreo + "', '" + usuNombreUsuario + "',  '" + usuContrasena + "')", cnn);
                comando.ExecuteNonQuery();
                
            }
            catch (SqlException){
                 
               mensaje = "Este nombre de usuario ya está tomado. Porfavor cambielo y vuelva a intentarlo";
            }
            cnn.Close();
            return mensaje;
        }

        public void LLenarGrid(DataGridView grid, string sql5)
        {
            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            comando = new SqlCommand(sql5, cnn);
            adapt = new SqlDataAdapter(comando);
            dt1 = new DataTable();
            adapt.Fill(dt1);
            grid.DataSource = dt1;
            cnn.Close();
        }

       
        public void verImagen(PictureBox pbIconoUsuario, string usuNombreUsuario) {
            try {
                if (cnn.State == ConnectionState.Closed)
                    cnn.Open();
                adapt = new SqlDataAdapter("Select usuFoto from TB_USUARIO where usuNombreUsuario = '" + usuNombreUsuario + "' ", cnn);
                ds = new DataSet();
                adapt.Fill(ds, "TB_USUARIO");
                byte[] datos = new byte[0];
                dr = ds.Tables["TB_USUARIO"].Rows[0];
                datos = (byte[])dr["usuFoto"];
                System.IO.MemoryStream ms = new System.IO.MemoryStream(datos);
                pbIconoUsuario.Image = System.Drawing.Bitmap.FromStream(ms);
            }
            catch (Exception e) {

                pbIconoUsuario.Image = pbIconoUsuario.InitialImage;
            }
            cnn.Close();
        }

        public void verClientes(DataGridView grid, String tipoUsuario) {

            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            comando = new SqlCommand("select dt.detperID, dt.detperCedulaRuc, dt.detperNombre, dt.detperApellido, dt.detperDireccion, dt.detperCorreo from TB_PERFILES p left join TB_DETALLE_PERFIL dt on p.perID = dt.perID where perDescripcion = '" + tipoUsuario + "';", cnn);
            adapt = new SqlDataAdapter(comando);
            dt1 = new DataTable();
            adapt.Fill(dt1);
            grid.DataSource = dt1;
            cnn.Close();

        }

        public string guardarImagen(PictureBox usuImagen, string usuNombreUsuario)
        {
            string mensaje = "Tu foto de perfil se actualizo";
            try
            {
                
                cnn.Open();
                comando = new SqlCommand("update TB_USUARIO set usuFoto = @usuImagen where  usuNombreUsuario = '" + usuNombreUsuario + "'", cnn);
                comando.Parameters.Add("@usuImagen", SqlDbType.Image);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                usuImagen.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                comando.Parameters["@usuImagen"].Value = ms.GetBuffer();
                comando.ExecuteNonQuery();
                cnn.Close();
            }
            catch (Exception ex)
            {
                mensaje = "Tu foto de perfil no se actualizo ";
            }
            return mensaje;
        }

        public void buscarComboboxUsuario(ComboBox comboBox, int tipoPerfil) {
            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            comando = new SqlCommand("select detperID , CONCAT(detperNombre, ' ', detperApellido) as Nombre from TB_DETALLE_PERFIL  where perID = '" + tipoPerfil + "'", cnn);
            adapt = new SqlDataAdapter(comando);
            dt1 = new DataTable();
            adapt.Fill(dt1);
            comboBox.ValueMember = "detperID";
            comboBox.DisplayMember = "Nombre";
            comboBox.DataSource = dt1;

            AutoCompleteStringCollection coleccion = new AutoCompleteStringCollection();
            foreach (DataRow row in dt1.Rows) {
                coleccion.Add(Convert.ToString(row["Nombre"]));
            }
            comboBox.AutoCompleteCustomSource = coleccion;
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            cnn.Close();
        }

        public void cargarDatos(ComboBox comboBox, TextBox cedula, TextBox correo)
        {
            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            comando = new SqlCommand("select detperCedulaRuc, detperCorreo from TB_DETALLE_PERFIL  where detperID = '" + comboBox.SelectedValue + "'", cnn);
            adapt = new SqlDataAdapter(comando);
            DataTable dt2 = new DataTable();
            adapt.Fill(dt2);
            foreach (DataRow dr in dt2.Rows) {

                cedula.Text = Convert.ToString(dr["detperCedulaRuc"]);
                correo.Text = Convert.ToString(dr["detperCorreo"]);
            }
            cnn.Close();
        }

        public static int usuariologeado;

        public string IngresarCompraVenta(ComboBox usuID, int tiptranID, string tranDescripcion, string tranFecha, string tranAutorizacion, string tranFechaVencimiento, string detranDescripcion, string detranPrecioUnitario, int detranCantidad) {
            string mensaje = "Registro guardado con exito";
                    
            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            try
            {
            comando = new SqlCommand("exec SP_ingreoCompraVenta '" + usuID.SelectedValue + "','" + usuariologeado + "','" + tiptranID + "','" + tranDescripcion + "', '" + Convert.ToDateTime(tranFecha).ToString("yyyy-MM-dd") + "' , '" + tranAutorizacion + "' ,'" + Convert.ToDateTime(tranFechaVencimiento).ToString("yyyy-MM-dd") + "', '" + detranDescripcion + "' , '" + detranPrecioUnitario + "' ,'" + detranCantidad + "' ", cnn);
            comando.ExecuteNonQuery();
            }
            catch (SqlException)
            {

                mensaje = "Ocurrió un error al ingresar el registro";
            }
            cnn.Close();
            return mensaje;
        }
        
        public string actualizarCompraVenta(ComboBox detperID, int tiptranID, string tranDescripcion, string tranFecha, string tranAutorizacion, string tranFechaVencimiento, string detranDescripcion, string detranPrecioUnitario, int detranCantidad, int tranID) {
            string mensaje = "Registro actualizado correctamente";
            try
            {
                if (cnn.State == ConnectionState.Closed)
                    cnn.Open();
                comando = new SqlCommand("exec SP_editarCompraVenta '" + detperID.SelectedValue + "','" + usuariologeado + "','" + tiptranID + "', '" + tranDescripcion + "', '" + tranFecha + "', '" + tranAutorizacion + "', '" + tranFechaVencimiento + "', '" + detranDescripcion + "', '" + detranPrecioUnitario + "', '" + detranCantidad + "', '" + tranID + "' ", cnn);
                comando.ExecuteNonQuery();
            }
            catch (SqlException) {
                mensaje = "Ocurrió un error al editar el registro";
            }
                return mensaje;
        }

        public void verCompraVenta(DataGridView grid, int tipoTransaccion)
        {

            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            comando = new SqlCommand("select t.tranID,dp.detperCedulaRuc, CONCAT(dp.detperApellido,' ',dp.detperNombre) as Apellido_Nombre,t.tranAutorizacion,dt.detranDescripcion, t.tranDescripcion, dt.detranCantidad, dt.detranPrecioUnitario,t.tranFecha,t.tranFechaVencimiento, (dt.detranPrecioUnitario * dt.detranCantidad) as Total from TB_DETALLE_PERFIL dp left join TB_TRANSACCION t on dp.detperID = t.detperID  inner join TB_DETALLES_TRANSACCION dt on t.tranID = dt.tranID where t.tiptranID = '" + tipoTransaccion + "' ; ", cnn);
            adapt = new SqlDataAdapter(comando);
            dt1 = new DataTable();
            adapt.Fill(dt1);
            grid.DataSource = dt1;
            cnn.Close();

        }

        public string eliminarCompraVenta(int tranID)
        {
            string mensaje = "Registro eliminado correctamente";
            try
            {
                if (cnn.State == ConnectionState.Closed)
                    cnn.Open();
                comando = new SqlCommand("exec SP_eliminarCompraVenta '" + tranID + "' ", cnn);
                comando.ExecuteNonQuery();
            }
            catch (SqlException)
            {
                mensaje = "Ocurrió un error al eliminar el registro";
            }
            return mensaje;
        }
    }
}