using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    // Repositorio encargado de las consultas SQL del perfil.
    // Trabaja con datos personales y cambio de contraseña del usuario conectado.
    public class PerfilRepository
    {
        // Cadena de conexión definida en Web.config.
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #region Obtener datos del perfil

        // Obtiene los datos básicos del usuario conectado.
        public PerfilViewModel ObtenerDatosPerfil(int usuarioId)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se consultan solo los campos que se muestran y editan en el perfil.
                string query = @"
                    SELECT Id, NombreCompleto, Correo
                    FROM Usuarios
                    WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                // Id del usuario que viene desde la sesión.
                cmd.Parameters.AddWithValue("@Id", usuarioId);

                con.Open();

                // ExecuteReader se usa porque esta consulta devuelve una fila con datos.
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Si encuentra al usuario, arma el ViewModel para la vista.
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

        // Revisa si el correo escrito ya pertenece a otro usuario.
        public bool ExisteCorreoEnOtroUsuario(int usuarioId, string correo)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Busca el mismo correo en usuarios distintos al usuario actual.
                string query = @"
                    SELECT COUNT(*)
                    FROM Usuarios
                    WHERE Correo = @Correo
                    AND Id <> @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Id", usuarioId);

                con.Open();

                // ExecuteScalar se usa porque la consulta devuelve un solo valor: COUNT(*).
                int cantidad = (int)cmd.ExecuteScalar();

                // Si cantidad es mayor a cero, el correo ya está ocupado por otro usuario.
                return cantidad > 0;
            }
        }

        // Actualiza nombre completo y correo del usuario conectado.
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

                // ExecuteNonQuery se usa porque esta consulta modifica datos.
                int filasAfectadas = cmd.ExecuteNonQuery();

                // Si modificó una fila, la actualización fue correcta.
                return filasAfectadas > 0;
            }
        }

        #endregion

        #region Validar contraseña actual

        // Revisa si la contraseña actual escrita coincide con la guardada en la base.
        public bool ValidarClaveActual(int usuarioId, string claveActual)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se cuenta si existe un usuario con ese Id y esa contraseña.
                string query = @"
                    SELECT COUNT(*)
                    FROM Usuarios
                    WHERE Id = @Id
                    AND ClaveHash = @ClaveActual";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", usuarioId);
                cmd.Parameters.AddWithValue("@ClaveActual", claveActual);

                con.Open();

                // Devuelve cuántos registros coinciden.
                int cantidad = (int)cmd.ExecuteScalar();

                // Si encuentra al menos uno, la contraseña actual es correcta.
                return cantidad > 0;
            }
        }

        #endregion

        #region Actualizar contraseña

        // Guarda la nueva contraseña del usuario conectado.
        public bool ActualizarContrasena(int usuarioId, string nuevaClave)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Aunque el campo se llama ClaveHash, en esta versión académica se guarda texto simple.
                // En un sistema real debería guardarse con hash seguro.
                string query = @"
                    UPDATE Usuarios
                    SET ClaveHash = @NuevaClave
                    WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@NuevaClave", nuevaClave);
                cmd.Parameters.AddWithValue("@Id", usuarioId);

                con.Open();

                // ExecuteNonQuery ejecuta el UPDATE y devuelve filas afectadas.
                int filasAfectadas = cmd.ExecuteNonQuery();

                // Si se modificó una fila, la contraseña fue actualizada.
                return filasAfectadas > 0;
            }
        }

        #endregion
    }
}