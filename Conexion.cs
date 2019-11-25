using System.Collections.Generic;
using System.Configuration;

namespace CapaDatos
{
    public class Conexion
    {
        private static GDatos _GDatos;

        public static GDatos GDatos
        {
            get

            {
                if (_GDatos != null)
                {
                    return _GDatos;
                }
                else
                {
                    //Conexion por defecto
                    _GDatos = new SqlServer(ConfigurationManager.AppSettings["Conexion"].ToString());
                }
                return _GDatos;
            }

            set
            {
                _GDatos = value;
            }
        }

        public static void ConexionPorDefault()
        {
            _GDatos = null;
        }

        public static void ConexionSMS(byte IdPais)
        {
            _GDatos = null;

            System.Data.DataTable consulta = new System.Data.DataTable();
            consulta = GDatos.ObtenerDataTableSql(string.Format("SELECT ServerSMS, BDSMS, ServerTRX, BDTRX, UsrSMS, PassSMS, UsrTrx, PassTrx FROM AppDesktop.MstBDConfiguracion WHERE IdPais={0}", IdPais));
            GDatos.CerrarConexion();
          
            string ServerSMS = consulta.Rows[0][0].ToString();
            string BDSMS = consulta.Rows[0][1].ToString().Replace("VAS_SmsAlertas", "VAS_SMS_NI");
            string UsrSMS = consulta.Rows[0][4].ToString();
            string PassSMS = consulta.Rows[0][5].ToString();

            _GDatos = new SqlServer(ServerSMS, BDSMS, UsrSMS, PassSMS);
        }

        public static void ConexionTRX(byte Idpais)
        {
            _GDatos = null;

            System.Data.DataTable consulta = new System.Data.DataTable();
            consulta = GDatos.ObtenerDataTableSql(string.Format("SELECT ServerSMS, BDSMS, ServerTRX, BDTRX, UsrSMS, PassSMS, UsrTrx, PassTrx FROM AppDesktop.MstBDConfiguracion WHERE IdPais={0}", Idpais));
            GDatos.CerrarConexion();

            string ServerTRX = consulta.Rows[0][2].ToString();
            string BDTRX = consulta.Rows[0][3].ToString();
            string UsrTrx = consulta.Rows[0][5].ToString();
            string PassTrx = consulta.Rows[7][6].ToString();

            _GDatos = new SqlServer(ServerTRX, BDTRX, UsrTrx, PassTrx);

        }

    }
}
