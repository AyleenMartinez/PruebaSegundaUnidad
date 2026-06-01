using System.Configuration;
using System.Data.SqlClient;

namespace PruebaSegundaUnidad.Repositories
{
    /// Administra las operaciones sensibles y exclusivas del usuario autenticado,
    /// enfocándose en la gestión y seguridad de su perfil.
    public class PerfilRepository
    {
        // Se obtiene la cadena de conexión desde el archivo Web.config
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #region Validar contraseña actual

        /// Verifica si la contraseña actual ingresada coincide con la contraseña guardada en la base de datos.
        /// <param name="usuarioId">Identificador del usuario que tiene la sesión iniciada.</param>
        /// <param name="claveActual">Contraseña actual escrita por el usuario en el formulario.</param>
        /// Retorna true si la contraseña coincide con el usuario.
        public bool ValidarClaveActual(int usuarioId, string claveActual)
        {
            // Se abre una conexión con la base de datos usando la cadena de conexión
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se cuenta si existe un usuario con ese Id y esa contraseña
                string query = "SELECT COUNT(*) FROM Usuarios WHERE Id = @Id AND ClaveHash = @ClaveActual";
                // Se prepara el comando SQL que se ejecutará
                SqlCommand cmd = new SqlCommand(query, con);

                // Se envía el Id del usuario como parámetro para evitar SQL Injection
                cmd.Parameters.AddWithValue("@Id", usuarioId);

                // Se envía la contraseña actual como parámetro
                cmd.Parameters.AddWithValue("@ClaveActual", claveActual);

                // Se abre la conexión antes de ejecutar la consulta
                con.Open();

                // ExecuteScalar devuelve un único valor, en este caso el COUNT(*)
                int cantidad = (int)cmd.ExecuteScalar();

                // Si cantidad es mayor a 0, significa que la contraseña actual sí coincide
                return cantidad > 0;
            }
        }

        #endregion

        #region Actualizar contraseña

        /// Actualiza la contraseña de un usuario específico en la base de datos.
        /// <param name="usuarioId">Identificador del usuario que tiene la sesión iniciada.</param>
        /// <param name="nuevaClaveHash">Nueva contraseña que se guardará en el campo ClaveHash.</param>
        /// Retorna true si al menos una fila fue modificada correctamente.
        public bool ActualizarContrasena(int usuarioId, string nuevaClaveHash)
        {
            // Se crea la conexión con la base de datos
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se actualiza la contraseña solo del usuario que corresponde al Id recibido
                string query = "UPDATE Usuarios SET ClaveHash = @NuevaClave WHERE Id = @Id";
                // Se prepara el comando SQL con la consulta anterior
                SqlCommand cmd = new SqlCommand(query, con);

                // Se envía la nueva contraseña como parámetro
                cmd.Parameters.AddWithValue("@NuevaClave", nuevaClaveHash);

                // Se envía el Id del usuario como parámetro
                cmd.Parameters.AddWithValue("@Id", usuarioId);

                // Se abre la conexión con la base de datos
                con.Open();

                // ExecuteNonQuery ejecuta INSERT, UPDATE o DELETE y devuelve las filas afectadas
                int filasAfectadas = cmd.ExecuteNonQuery();

                // Si se modificó al menos una fila, la actualización fue correcta
                return filasAfectadas > 0;
            }
        }

        #endregion
    }
}