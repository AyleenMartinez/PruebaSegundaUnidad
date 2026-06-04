using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    // Repositorio encargado de las consultas SQL para usuarios.
    // Se usa desde UsuariosController.
    public class UsuarioRepository
    {
        // Cadena de conexión guardada en Web.config.
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #region Listar usuarios

        // Obtiene todos los usuarios junto con su rol.
        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT 
                        U.Id,
                        U.RolId,
                        R.NombreRol,
                        U.NombreCompleto,
                        U.NombreUsuario,
                        U.Correo,
                        U.Estado,
                        U.FechaRegistro
                    FROM Usuarios U
                    INNER JOIN Roles R ON U.RolId = R.Id
                    ORDER BY U.FechaRegistro ASC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
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
            }

            return lista;
        }

        #endregion

        #region Validaciones de duplicados

        // Revisa si existe otro usuario con el mismo correo.
        public bool ExisteCorreo(string correo, int? usuarioIdExcluir = null)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM Usuarios
                    WHERE Correo = @Correo";

                if (usuarioIdExcluir != null)
                {
                    query += " AND Id <> @Id";
                }

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Correo", correo);

                if (usuarioIdExcluir != null)
                {
                    cmd.Parameters.AddWithValue("@Id", usuarioIdExcluir.Value);
                }

                con.Open();

                int cantidad = (int)cmd.ExecuteScalar();

                return cantidad > 0;
            }
        }

        // Revisa si existe otro usuario con el mismo nombre de usuario.
        public bool ExisteNombreUsuario(string nombreUsuario, int? usuarioIdExcluir = null)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM Usuarios
                    WHERE NombreUsuario = @NombreUsuario";

                if (usuarioIdExcluir != null)
                {
                    query += " AND Id <> @Id";
                }

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                if (usuarioIdExcluir != null)
                {
                    cmd.Parameters.AddWithValue("@Id", usuarioIdExcluir.Value);
                }

                con.Open();

                int cantidad = (int)cmd.ExecuteScalar();

                return cantidad > 0;
            }
        }

        #endregion

        #region Crear usuario

        // Inserta un usuario nuevo en la base.
        public void Insertar(Usuario usuario)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    INSERT INTO Usuarios
                    (
                        RolId,
                        NombreCompleto,
                        NombreUsuario,
                        Correo,
                        ClaveHash,
                        Estado
                    )
                    VALUES
                    (
                        @RolId,
                        @NombreCompleto,
                        @NombreUsuario,
                        @Correo,
                        @ClaveHash,
                        @Estado
                    )";

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

        #endregion

        #region Obtener usuario por Id

        // Busca un usuario por su Id.
        public Usuario ObtenerPorId(int id)
        {
            Usuario usuario = null;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT 
                        Id,
                        RolId,
                        NombreCompleto,
                        NombreUsuario,
                        Correo,
                        Estado
                    FROM Usuarios
                    WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
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
            }

            return usuario;
        }

        #endregion

        #region Editar usuario

        // Actualiza los datos de un usuario existente.
        // La contraseña no se cambia desde este método.
        public void Actualizar(Usuario usuario)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    UPDATE Usuarios
                    SET RolId = @RolId,
                        NombreCompleto = @NombreCompleto,
                        NombreUsuario = @NombreUsuario,
                        Correo = @Correo,
                        Estado = @Estado
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

        #endregion

        #region Cambiar estado

        // Cambia el estado del usuario.
        // Si está activo, pasa a inactivo; si está inactivo, pasa a activo.
        public void CambiarEstado(int id)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    UPDATE Usuarios
                    SET Estado = CASE 
                                    WHEN Estado = 1 THEN 0 
                                    ELSE 1 
                                 END
                    WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}