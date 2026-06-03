using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    /// Administra las operaciones sensibles y exclusivas del usuario autenticado,
    /// enfocándose en la gestión y seguridad de su perfil.
    public class PerfilRepository
    {
        // Se obtiene la cadena de conexión desde el archivo Web.config.
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #region Obtener datos del perfil

        /// Obtiene los datos básicos del usuario autenticado desde la base de datos.
        /// Antes: el perfil se cargaba solo desde Session.
        /// Ahora: se puede volver a consultar la base para tener datos actualizados.
        public PerfilViewModel ObtenerDatosPerfil(int usuarioId)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se buscan solo los datos básicos que se muestran y editan en el perfil.
                string query = @"
                    SELECT Id, NombreCompleto, Correo
                    FROM Usuarios
                    WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                // Id del usuario que tiene la sesión iniciada.
                cmd.Parameters.AddWithValue("@Id", usuarioId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Si encuentra al usuario, se arma el ViewModel del perfil.
                    if (reader.Read())
                    {
                        return new PerfilViewModel
                        {
                            UsuarioId = (int)reader["Id"],
                            NombreCompleto = reader["NombreCompleto"].ToString(),
                            Correo = reader["Correo"].ToString()
                        };
                    }
                }
            }

            // Si no encuentra usuario, devuelve null.
            return null;
        }

        #endregion

        #region Actualizar datos personales

        /// Revisa si el correo ya está usado por otro usuario.
        /// Antes: no se editaba correo, entonces no se necesitaba esta validación.
        /// Ahora: se valida para no romper la restricción UNIQUE de la tabla Usuarios.
        public bool ExisteCorreoEnOtroUsuario(int usuarioId, string correo)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Busca correos iguales, pero excluye al usuario actual.
                string query = @"
                    SELECT COUNT(*)
                    FROM Usuarios
                    WHERE Correo = @Correo
                    AND Id <> @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Id", usuarioId);

                con.Open();

                int cantidad = (int)cmd.ExecuteScalar();

                // Si cantidad es mayor a 0, el correo pertenece a otro usuario.
                return cantidad > 0;
            }
        }

        /// Actualiza los datos básicos del perfil del usuario autenticado.
        /// Antes: el perfil solo mostraba nombre y correo.
        /// Ahora: permite guardar cambios de nombre completo y correo en la base.
        public bool ActualizarDatosPerfil(int usuarioId, string nombreCompleto, string correo)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se actualizan solo los datos básicos del usuario actual.
                string query = @"
                    UPDATE Usuarios
                    SET NombreCompleto = @NombreCompleto,
                        Correo = @Correo
                    WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@NombreCompleto", nombreCompleto);
                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Id", usuarioId);

                con.Open();

                int filasAfectadas = cmd.ExecuteNonQuery();

                // Si modificó una fila, el guardado fue correcto.
                return filasAfectadas > 0;
            }
        }

        #endregion

        #region Validar contraseña actual

        /// Verifica si la contraseña actual ingresada coincide con la contraseña guardada en la base de datos.
        public bool ValidarClaveActual(int usuarioId, string claveActual)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se cuenta si existe un usuario con ese Id y esa contraseña.
                string query = "SELECT COUNT(*) FROM Usuarios WHERE Id = @Id AND ClaveHash = @ClaveActual";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", usuarioId);
                cmd.Parameters.AddWithValue("@ClaveActual", claveActual);

                con.Open();

                int cantidad = (int)cmd.ExecuteScalar();

                // Si cantidad es mayor a 0, significa que la contraseña actual sí coincide.
                return cantidad > 0;
            }
        }

        #endregion

        #region Actualizar contraseña

        /// Actualiza la contraseña de un usuario específico en la base de datos.
        public bool ActualizarContrasena(int usuarioId, string nuevaClaveHash)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se actualiza la contraseña solo del usuario que corresponde al Id recibido.
                string query = "UPDATE Usuarios SET ClaveHash = @NuevaClave WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@NuevaClave", nuevaClaveHash);
                cmd.Parameters.AddWithValue("@Id", usuarioId);

                con.Open();

                int filasAfectadas = cmd.ExecuteNonQuery();

                // Si se modificó al menos una fila, la actualización fue correcta.
                return filasAfectadas > 0;
            }
        }

        #endregion
    }
}