using System;
using System.Configuration;
using System.Data.SqlClient;

namespace PruebaSegundaUnidad.Repositories
{
    /// Administra las operaciones sensibles y exclusivas del usuario autenticado, 
    /// enfocándose en la gestión y seguridad de su perfil.
    public class PerfilRepository
    {
        // Obtiene la cadena de conexión desde el archivo de configuración (Web.config)
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        /// Actualiza la contraseña encriptada de un usuario específico en la base de datos.
        /// <param name="usuarioId">El identificador único del usuario en sesión.</param>
        /// <param name="nuevaClaveHash">La nueva contraseña previamente procesada/hasheada desde el controlador.</param>
        /// Retorna 'true' si al menos una fila fue modificada exitosamente en la base de datos; 
        /// de lo contrario, retorna 'false'.
        public bool ActualizarContrasena(int usuarioId, string nuevaClaveHash)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Consulta SQL de actualización directa utilizando parámetros de seguridad
                string query = "UPDATE Usuarios SET ClaveHash = @NuevaClave WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@NuevaClave", nuevaClaveHash);
                cmd.Parameters.AddWithValue("@Id", usuarioId);

                con.Open();

                // ExecuteNonQuery devuelve el número de filas afectadas por el UPDATE
                int filasAfectadas = cmd.ExecuteNonQuery();

                // Si filasAfectadas es mayor a 0, la operación fue un éxito
                return filasAfectadas > 0;
            }
        }
    }
}