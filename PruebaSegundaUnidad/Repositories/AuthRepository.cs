using System;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    /// Repositorio encargado de gestionar las operaciones de base de datos 
    /// exclusivas del flujo de autenticación.
    /// Separa completamente la lógica de SQL de los controladores MVC.
    public class AuthRepository
    {
        #region Conexion

        // Verifica que el nombre "ConexionSQL" sea exactamente el mismo 
        // atributo 'name' definido en la sección <connectionStrings> de tu Web.config.
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #endregion

        #region Validacion de login

        /// Busca un usuario activo en la base de datos comparando las credenciales ingresadas.
        /// <param name="usuarioOCorreo">Dato ingresado en el formulario (soporta Username o Email).</param>
        /// <param name="clave">Contraseña del usuario (idealmente pre-procesada como Hash desde el controlador).</param>
        /// <returns>Retorna un objeto Usuario poblado si el login es exitoso; de lo contrario, retorna null.</returns>
        public Usuario ValidarLogin(string usuarioOCorreo, string clave)
        {
            Usuario usuario = null;

            // El bloque 'using' asegura la liberación automática de los recursos de la conexión (con.Dispose).
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se utiliza INNER JOIN para cargar la propiedad 'NombreRol' de una sola vez.
                // La condición 'Estado = 1' impide directamente el acceso a cuentas desactivadas.
                string query = @"
                    SELECT 
                        U.Id,
                        U.RolId,
                        R.NombreRol,
                        U.NombreCompleto,
                        U.NombreUsuario,
                        U.Correo,
                        U.ClaveHash,
                        U.Estado,
                        U.FechaRegistro
                    FROM Usuarios U
                    INNER JOIN Roles R ON U.RolId = R.Id
                    WHERE 
                        (U.NombreUsuario = @usuarioOCorreo OR U.Correo = @usuarioOCorreo)
                        AND U.ClaveHash = @clave
                        AND U.Estado = 1";

                SqlCommand cmd = new SqlCommand(query, con);

                // Parametrización estricta para evitar vulnerabilidades de Inyección SQL.
                cmd.Parameters.AddWithValue("@usuarioOCorreo", usuarioOCorreo);
                cmd.Parameters.AddWithValue("@clave", clave);

                con.Open();

                // SqlDataReader es el método más rápido (solo avance y lectura) para recuperar filas.
                SqlDataReader reader = cmd.ExecuteReader();

                // Si reader.Read() devuelve true, se encontró una coincidencia exacta en la BD.
                if (reader.Read())
                {
                    usuario = new Usuario
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        RolId = Convert.ToInt32(reader["RolId"]),
                        NombreRol = reader["NombreRol"].ToString(),
                        NombreCompleto = reader["NombreCompleto"].ToString(),
                        NombreUsuario = reader["NombreUsuario"].ToString(),
                        Correo = reader["Correo"].ToString(),
                        ClaveHash = reader["ClaveHash"].ToString(),
                        Estado = Convert.ToBoolean(reader["Estado"]),
                        FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
                    };
                }
            }

            // El controlador evaluará esto: si es null, muestra error; si tiene datos, crea la sesión.
            return usuario;
        }

        #endregion
    }
}