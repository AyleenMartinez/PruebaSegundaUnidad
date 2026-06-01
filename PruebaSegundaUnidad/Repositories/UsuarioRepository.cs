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

        /// <summary>
        /// Invierte el estado actual del usuario (Activo <-> Inactivo).
        /// </summary>
        public void CambiarEstado(int id)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // La consulta SQL invierte el bit: si es 1 pasa a 0, si es 0 pasa a 1.
                string query = "UPDATE Usuarios SET Estado = ~Estado WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID para editarlo.
        /// </summary>
        public Usuario ObtenerPorId(int id)
        {
            Usuario usuario = null;
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = "SELECT Id, RolId, NombreCompleto, NombreUsuario, Correo, Estado FROM Usuarios WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    usuario = new Usuario
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        RolId = Convert.ToInt32(reader["RolId"]),
                        NombreCompleto = reader["NombreCompleto"].ToString(),
                        NombreUsuario = reader["NombreUsuario"].ToString(),
                        Correo = reader["Correo"].ToString(),
                        Estado = Convert.ToBoolean(reader["Estado"])
                    };
                }
            }
            return usuario;
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// </summary>
        public void Actualizar(Usuario usuario)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Nota: No actualizamos la contraseña aquí por seguridad.
                string query = @"UPDATE Usuarios 
                                 SET RolId = @RolId, NombreCompleto = @NombreCompleto, 
                                     NombreUsuario = @NombreUsuario, Correo = @Correo, Estado = @Estado 
                                 WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@RolId", usuario.RolId);
                cmd.Parameters.AddWithValue("@NombreCompleto", usuario.NombreCompleto);
                cmd.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                cmd.Parameters.AddWithValue("@Estado", usuario.Estado);
                cmd.Parameters.AddWithValue("@Id", usuario.Id);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}