using System;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    // Repositorio del login.
    // Aquí se hacen las consultas a la BD, no se decide qué vista mostrar.
    public class AuthRepository
    {
        #region Conexion

        // Se toma la conexión guardada en Web.config con el nombre ConexionSQL.
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #endregion

        #region Validacion de login

        // Busca un usuario activo usando correo o nombre de usuario.
        public Usuario ValidarLogin(string usuarioOCorreo, string clave)
        {
            Usuario usuario = null;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se consulta Usuarios y Roles para traer también el nombre del rol.
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

                // Parametros para evitar escribir los valores directo dentro del SQL.
                cmd.Parameters.AddWithValue("@usuarioOCorreo", usuarioOCorreo);
                cmd.Parameters.AddWithValue("@clave", clave);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                // Si encontró una fila, se arma el objeto Usuario.
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

            // Si no encontró nada, usuario vuelve como null.
            return usuario;
        }

        #endregion
    }
}