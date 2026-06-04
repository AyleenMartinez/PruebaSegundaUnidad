using System;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    // Repositorio encargado de las consultas SQL del inicio de sesión.
    public class AuthRepository
    {
        // Cadena de conexión definida en Web.config.
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #region Buscar usuario para login

        // Busca una cuenta usando nombre de usuario o correo.
        // Solo busca si la cuenta existe; la contraseña y el estado se revisan en el controlador.
        public Usuario ObtenerPorUsuarioOCorreo(string usuarioOCorreo)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Consulta la tabla Usuarios y la relaciona con Roles para obtener el nombre del rol.
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
                    WHERE U.NombreUsuario = @UsuarioOCorreo
                       OR U.Correo = @UsuarioOCorreo";

                SqlCommand cmd = new SqlCommand(query, con);

                // Parámetro que recibe lo escrito en el campo Usuario o Correo.
                cmd.Parameters.AddWithValue("@UsuarioOCorreo", usuarioOCorreo);

                // Abre la conexión antes de ejecutar la consulta.
                con.Open();

                // ExecuteReader se usa porque esta consulta devuelve filas desde la base de datos.
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Si encuentra un usuario, se arma el objeto Usuario.
                    if (reader.Read())
                    {
                        return new Usuario
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
            }

            // Si no encuentra usuario ni correo, devuelve null.
            return null;
        }

        #endregion
    }
}