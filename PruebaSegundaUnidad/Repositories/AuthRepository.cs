using System;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    // Repositorio encargado de consultar usuarios para el inicio de sesión.
    public class AuthRepository
    {
        // Cadena de conexión definida en Web.config.
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #region Buscar usuario para login

        // Busca una cuenta usando nombre de usuario o correo.
        // Aquí no se valida contraseña ni estado, solo se busca si la cuenta existe.
        public Usuario ObtenerPorUsuarioOCorreo(string usuarioOCorreo)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Consulta el usuario junto con su rol.
                // Se permite buscar por NombreUsuario o por Correo.
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

                // Parámetro recibido desde el formulario de login.
                cmd.Parameters.AddWithValue("@UsuarioOCorreo", usuarioOCorreo);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Si encuentra una fila, arma el objeto Usuario.
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

            // Si no encuentra usuario o correo, devuelve null.
            return null;
        }

        #endregion
    }
}