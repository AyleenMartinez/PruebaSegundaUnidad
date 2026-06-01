using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    /// <summary>
    /// Gestiona el CRUD completo de la tabla Usuarios para el panel de administración (MVC tradicional).
    /// </summary>
    public class UsuarioRepository
    {
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        /// <summary>
        /// Lista todos los usuarios registrados incluyendo el nombre de su rol.
        /// </summary>
        /// <returns>Lista de objetos Usuario poblada desde la base de datos.</returns>
        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> lista = new List<Usuario>();
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // La consulta coincide perfectamente con las columnas de las tablas Usuarios y Roles
                string query = @"
                    SELECT U.Id, U.RolId, R.NombreRol, U.NombreCompleto, U.NombreUsuario, U.Correo, U.Estado, U.FechaRegistro 
                    FROM Usuarios U
                    INNER JOIN Roles R ON U.RolId = R.Id";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Usuario
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        RolId = Convert.ToInt32(reader["RolId"]),
                        NombreRol = reader["NombreRol"].ToString(),
                        NombreCompleto = reader["NombreCompleto"].ToString(),
                        NombreUsuario = reader["NombreUsuario"].ToString(),
                        Correo = reader["Correo"].ToString(),
                        Estado = Convert.ToBoolean(reader["Estado"]),
                        FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
                    });
                }
            }
            return lista;
        }

        /// <summary>
        /// Registra un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="usuario">Objeto con los datos del nuevo usuario ingresados en el formulario.</param>
        public void Insertar(Usuario usuario)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // El campo FechaRegistro se omite para que la BD aplique el DEFAULT GETDATE()
                string query = @"INSERT INTO Usuarios (RolId, NombreCompleto, NombreUsuario, Correo, ClaveHash, Estado) 
                                 VALUES (@RolId, @NombreCompleto, @NombreUsuario, @Correo, @ClaveHash, @Estado)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@RolId", usuario.RolId);
                cmd.Parameters.AddWithValue("@NombreCompleto", usuario.NombreCompleto);
                cmd.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                cmd.Parameters.AddWithValue("@ClaveHash", usuario.ClaveHash);
                cmd.Parameters.AddWithValue("@Estado", usuario.Estado);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}