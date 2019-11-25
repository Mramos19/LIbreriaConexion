using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class SqlServer : GDatos
    {
        //Contructor de la clase SqlServer
        public SqlServer()
        {
            _Servidor = "";
            _BaseDatos = "";
            _Usuario = "";
            _Password = "";
            _CadenaConexion = "";
        }

        //Contructor sobrecargado que recibe la conexión para luego establecerla en el metodo set de la variable _CadenaConexion(public override sealed string CadenaConexion)
        public SqlServer(string cadenaConexion)
        {
            CadenaConexion = cadenaConexion;
        }

        //Contructor sobrecargado que recibe la conexión para luego establecerla en el metodo get de la variable _CadenaConexion (public override sealed string CadenaConexion)
        public SqlServer(string Servidor, string BaseDatos, string Usuario, string Password)
        {
            _Servidor = Servidor;
            _BaseDatos = BaseDatos;
            _Usuario = Usuario;
            _Password = Password;
            _CadenaConexion = "";
            Autenticar();
        }

        #region Funciones

        //Funcion Privada para obtener un objeto generico a partir de un dataRow
        private T GetItem<T>(System.Data.DataRow dr)
        {
            //La variable temp contiene el tipo de objeto, esto para poder obtener la informacion del objeto generico obtenida en la funcion
            Type temp = typeof(T);

            //Se crea una instancia del objeto generico recibida en la funcion
            T obj = Activator.CreateInstance<T>();

            //Recorremos la columna de cada fila para compara los atributos del objeto generico y las colunas de cada fila
            foreach (System.Data.DataColumn column in dr.Table.Columns)
            {
                //obtenemos informacion de cada atributo o elementos del objeto generico
                foreach (System.Reflection.PropertyInfo pro in temp.GetProperties())
                {
                    //si el tipo de cada atributo de la instancia del objeto generico es igual a cada columna
                    //se establecen los valores respectivos, en caso contrario sigue comparando cada atributo
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }

            //se retorna el objeto con los atributos establecidos
            return obj;
        }

        #endregion

        //Variable para establecer y obtener el valor de la variable _CadenaConexion
        public override sealed string CadenaConexion
        {
            get
            {
                //si el length es cero es porque se usa el contructor (public SqlServer(string Servidor, string BaseDatos, string Usuario, string Password))
                if (_CadenaConexion.Length == 0)
                {
                    //Estableciendo la cadena conexión
                    if (_Servidor.Length != 0 && _BaseDatos.Length != 0 && _Usuario.Length != 0 && _Password.Length != 0)
                    {
                        var sCadena = new System.Text.StringBuilder("");
                        sCadena.Append("data source=<SERVIDOR>;");
                        sCadena.Append("initial catalog=<BASE>;");
                        sCadena.Append("user id=<USER>;");
                        sCadena.Append("password=<PASSWORD>;");
                        sCadena.Append("persist security info=True;");
                        sCadena.Replace("<SERVIDOR>", _Servidor);
                        sCadena.Replace("<BASE>", _BaseDatos);
                        sCadena.Replace("<USER>", _Usuario);
                        sCadena.Replace("<PASSWORD>", _Password);

                        return sCadena.ToString();
                    }
                    throw new Exception("No se puede establecer la cadena de conexión en la clase DatosSQLServer");
                }
                else
                {   //si la cadena ya se habia establecido solo se retorna
                    return _CadenaConexion;
                }

            }
            set
            {
                //se establece el valor si usamos el contructor sobrecargado que para por parametro la cadena de conexion (public SqlServer(string cadenaConexion))
                _CadenaConexion = value;
            }
        }


        protected override IDbConnection CrearConexion(string CadenaConexion)
        {
            return new System.Data.SqlClient.SqlConnection(CadenaConexion);
        }

        public override int EjecutarSql(string ConsultaSql)
        {
            SqlCommand cmd = new SqlCommand(ConsultaSql, (SqlConnection)Conexion, (SqlTransaction)Transaccion);

            return cmd.ExecuteNonQuery();
        }

        public override int Ejecutar(string ProcedimientoAlmacenado)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;

            return cmd.ExecuteNonQuery();
        }

        public override int Ejecutar(string ProcedimientoAlmacenado, params object[] arg)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;
            SqlCommandBuilder.DeriveParameters(cmd);

            for (int i = 0; i < cmd.Parameters.Count - 1; i++)
            {
                cmd.Parameters[i + 1].Value = arg[i];
            }

            return cmd.ExecuteNonQuery();
        }

        public override object ObtenerValorScalarSql(string ConsultaSql)
        {
            SqlCommand cmd = new SqlCommand(ConsultaSql, (SqlConnection)Conexion, (SqlTransaction)Transaccion);

            return cmd.ExecuteScalar();
        }

        public override object ObtenerValorScalar(string ProcedimientoAlmacenado)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;

            return cmd.ExecuteScalar();
        }

        public override object ObtenerValorScalar(string ProcedimientoAlmacenado, params object[] arg)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;
            SqlCommandBuilder.DeriveParameters(cmd);

            for (int i = 0; i < cmd.Parameters.Count - 1; i++)
            {
                cmd.Parameters[i + 1].Value = arg[i];
            }

            return cmd.ExecuteScalar();

        }

        public override DataSet ObtenerDataSetSql(string ConsultaSql)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(ConsultaSql, (SqlConnection)Conexion, (SqlTransaction)Transaccion);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds;
        }

        public override DataSet ObtenerDataSet(string ProcedimientoAlmacenado)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds;
        }

        public override DataSet ObtenerDataSet(string ProcedimientoAlmacenado, params object[] arg)
        {

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;
            SqlCommandBuilder.DeriveParameters(cmd);

            for (int i = 0; i < cmd.Parameters.Count - 1; i++)
            {
                cmd.Parameters[i + 1].Value = arg[i];
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds;
        }

        public override DataTable ObtenerDataTableSql(string ConsultaSql)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(ConsultaSql, (SqlConnection)Conexion, (SqlTransaction)Transaccion);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds.Tables[0].Copy();
        }

        public override DataTable ObtenerDataTable(string ProcedimientoAlmacenado)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds.Tables[0].Copy();
        }

        public override DataTable ObtenerDataTable(string ProcedimientoAlmacenado, params object[] arg)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;
            SqlCommandBuilder.DeriveParameters(cmd);

            for (int i = 0; i < cmd.Parameters.Count - 1; i++)
            {
                cmd.Parameters[i + 1].Value = arg[i];
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds.Tables[0].Copy();
        }

        public override List<T> ObtenerLinqSql<T>(string ConsultaSql)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(ConsultaSql, (SqlConnection)Conexion);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            CerrarConexion();

            //Definimos la lista Generica que retornará el metodo
            List<T> data = new List<T>();

            //Recorremos cada una de las filas para obtener el objeto generico que se agregará a la lista
            foreach (System.Data.DataRow row in ds.Tables[0].Copy().Rows)
            {
                //Item es el elemento generico que retornara la funcion GetItem a partir de los elementos o columnas de la fila (Row del DataTable)
                T item = GetItem<T>(row);

                //Agregamos el objeto a la lista Generica
                data.Add(item);
            }

            //Se retorna la lista Generica
            return data;

        }

        public override List<T> ObtenerLinq<T>(string ProcedimientoAlmacenado)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            CerrarConexion();

            //Definimos la lista Generica que retornará el metodo
            List<T> data = new List<T>();

            //Recorremos cada una de las filas para obtener el objeto generico que se agregará a la lista
            foreach (System.Data.DataRow row in ds.Tables[0].Copy().Rows)
            {
                //Item es el elemento generico que retornara la funcion GetItem a partir de los elementos o columnas de la fila (Row del DataTable)
                T item = GetItem<T>(row);

                //Agregamos el objeto a la lista Generica
                data.Add(item);
            }

            //Se retorna la lista Generica
            return data;

        }

        public override List<T> ObtenerLinq<T>(string ProcedimientoAlmacenado, params object[] arg)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            SqlCommandBuilder.DeriveParameters(cmd);

            for (int i = 0; i < cmd.Parameters.Count - 1; i++)
            {
                cmd.Parameters[i + 1].Value = arg[i];
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            CerrarConexion();

            //Definimos la lista Generica que retornará el metodo
            List<T> data = new List<T>();

            //Recorremos cada una de las filas para obtener el objeto generico que se agregará a la lista
            foreach (System.Data.DataRow row in ds.Tables[0].Copy().Rows)
            {
                //Item es el elemento generico que retornara la funcion GetItem a partir de los elementos o columnas de la fila (Row del DataTable)
                T item = GetItem<T>(row);

                //Agregamos el objeto a la lista Generica
                data.Add(item);
            }

            //Se retorna la lista Generica
            return data;

        }

    }
}
