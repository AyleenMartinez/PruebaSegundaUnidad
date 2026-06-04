using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    // Repositorio encargado de las consultas SQL para usuarios.
    // Se usa desde UsuariosController para separar la lógica de base de datos.
    public class UsuarioRepository
    {
        // Cadena de conexión definida en Web.config.
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #region Listar usuarios

        // Obtiene todos los usuarios junto con el nombre de su rol.
        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se hace JOIN con Roles porque NombreRol no está directamente en la tabla Usuarios.
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

                // ExecuteReader se usa porque esta consulta devuelve varias filas.
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Se recorre cada fila devuelta por SQL Server.
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

        // Revisa si ya existe un usuario con el mismo correo.
        // Si usuarioIdExcluir tiene valor, excluye ese Id para permitir editar el mismo usuario.
        public bool ExisteCorreo(string correo, int? usuarioIdExcluir = null)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM Usuarios
                    WHERE Correo = @Correo";

                // Esta parte se usa al editar, para no comparar el usuario consigo mismo.
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

                // ExecuteScalar se usa porque el SELECT COUNT(*) devuelve un solo valor.
                int cantidad = (int)cmd.ExecuteScalar();

                return cantidad > 0;
            }
        }

        // Revisa si ya existe un usuario con el mismo nombre de usuario.
        // También permite excluir un Id cuando se está editando.
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

                // Devuelve cuántos usuarios existen con el mismo nombre de usuario.
                int cantidad = (int)cmd.ExecuteScalar();

                return cantidad > 0;
            }
        }

        #endregion

        #region Crear usuario

        // Inserta un usuario nuevo en la base de datos.
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

                // Se envían los valores como parámetros para evitar concatenar texto directo en SQL.
                cmd.Parameters.AddWithValue("@RolId", usuario.RolId);
                cmd.Parameters.AddWithValue("@NombreCompleto", usuario.NombreCompleto);
                cmd.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                cmd.Parameters.AddWithValue("@ClaveHash", usuario.ClaveHash);
                cmd.Parameters.AddWithValue("@Estado", usuario.Estado);

                con.Open();

                // ExecuteNonQuery se usa porque INSERT modifica la base y no devuelve tabla.
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Obtener usuario por Id

        // Busca un usuario específico por su Id.
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

                // ExecuteReader se usa porque se espera leer una fila con datos del usuario.
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

        // Actualiza los datos básicos de un usuario existente.
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

                // ExecuteNonQuery se usa porque UPDATE modifica datos.
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

                // ExecuteNonQuery se usa porque se actualiza el campo Estado.
                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}