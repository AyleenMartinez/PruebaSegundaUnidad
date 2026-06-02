using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    // Repositorio de solicitudes de soporte.
    // Aquí van las consultas SQL que consume la API.
    public class SolicitudRepository
    {
        #region Conexión

        // Se toma la conexión guardada en Web.config.
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #endregion

        #region Listar solicitudes

        // Lista todas las solicitudes con sus datos relacionados.
        public List<SolicitudSoporte> ObtenerTodas()
        {
            // Lista que se enviará de vuelta a la API.
            List<SolicitudSoporte> lista = new List<SolicitudSoporte>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Antes: una solicitud solo tendría Ids difíciles de leer.
                // Ahora: se usan INNER JOIN para traer nombres de usuario, área, prioridad y estado.
                string query = @"
                    SELECT 
                        S.Id,
                        S.UsuarioId,
                        U.NombreCompleto AS NombreUsuario,
                        U.Correo AS CorreoUsuario,
                        S.AreaSolicitanteId AS IdArea,
                        A.NombreArea,
                        S.TipoProblemaId AS IdTipoProblema,
                        T.NombreTipoProblema AS DescripcionProblema,
                        S.PrioridadId AS IdPrioridad,
                        P.NombrePrioridad AS NivelPrioridad,
                        S.EstadoSolicitudId AS IdEstado,
                        E.NombreEstado,
                        S.Descripcion,
                        S.FechaRegistro
                    FROM SolicitudesSoporte S
                    INNER JOIN Usuarios U ON S.UsuarioId = U.Id
                    INNER JOIN AreasSolicitantes A ON S.AreaSolicitanteId = A.Id
                    INNER JOIN TiposProblema T ON S.TipoProblemaId = T.Id
                    INNER JOIN Prioridades P ON S.PrioridadId = P.Id
                    INNER JOIN EstadosSolicitud E ON S.EstadoSolicitudId = E.Id
                    ORDER BY S.FechaRegistro DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Antes: el armado del objeto se repetía en varios métodos.
                        // Ahora: se usa MapearSolicitud para no duplicar tanto código.
                        lista.Add(MapearSolicitud(reader));
                    }
                }
            }

            return lista;
        }

        #endregion

        #region Buscar solicitud por Id

        // Busca una solicitud específica.
        public SolicitudSoporte ObtenerPorId(int id)
        {
            // Parte como null por si no se encuentra nada.
            SolicitudSoporte solicitud = null;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Antes: solo se listaban todas las solicitudes.
                // Ahora: también se puede buscar una específica usando WHERE S.Id = @Id.
                string query = @"
                    SELECT 
                        S.Id,
                        S.UsuarioId,
                        U.NombreCompleto AS NombreUsuario,
                        U.Correo AS CorreoUsuario,
                        S.AreaSolicitanteId AS IdArea,
                        A.NombreArea,
                        S.TipoProblemaId AS IdTipoProblema,
                        T.NombreTipoProblema AS DescripcionProblema,
                        S.PrioridadId AS IdPrioridad,
                        P.NombrePrioridad AS NivelPrioridad,
                        S.EstadoSolicitudId AS IdEstado,
                        E.NombreEstado,
                        S.Descripcion,
                        S.FechaRegistro
                    FROM SolicitudesSoporte S
                    INNER JOIN Usuarios U ON S.UsuarioId = U.Id
                    INNER JOIN AreasSolicitantes A ON S.AreaSolicitanteId = A.Id
                    INNER JOIN TiposProblema T ON S.TipoProblemaId = T.Id
                    INNER JOIN Prioridades P ON S.PrioridadId = P.Id
                    INNER JOIN EstadosSolicitud E ON S.EstadoSolicitudId = E.Id
                    WHERE S.Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                // Parámetro para buscar solo la solicitud indicada.
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Si encontró una fila, se convierte en objeto.
                        solicitud = MapearSolicitud(reader);
                    }
                }
            }

            // Si no encontró nada, vuelve null.
            return solicitud;
        }

        #endregion

        #region Crear solicitud

        // Inserta una nueva solicitud en la base.
        public bool Insertar(SolicitudSoporte solicitud)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Antes: podía no quedar claro el estado inicial de una solicitud nueva.
                // Ahora: toda solicitud nueva se guarda automáticamente como Pendiente con EstadoSolicitudId = 1.
                //
                // Antes: FechaRegistro se podía intentar enviar manualmente.
                // Ahora: FechaRegistro la llena la base con DEFAULT GETDATE().
                //
                // Nota: FechaSolicitud se llena con GETDATE() porque la tabla la pide como obligatoria.
                string query = @"
                    INSERT INTO SolicitudesSoporte
                    (
                        UsuarioId,
                        AreaSolicitanteId,
                        TipoProblemaId,
                        PrioridadId,
                        EstadoSolicitudId,
                        Descripcion,
                        FechaSolicitud
                    )
                    VALUES
                    (
                        @UsuarioId,
                        @IdArea,
                        @IdTipoProblema,
                        @IdPrioridad,
                        1,
                        @Descripcion,
                        GETDATE()
                    )";

                SqlCommand cmd = new SqlCommand(query, con);

                // Usuario que creó la solicitud.
                cmd.Parameters.AddWithValue("@UsuarioId", solicitud.UsuarioId);

                // Área seleccionada.
                cmd.Parameters.AddWithValue("@IdArea", solicitud.IdArea);

                // Tipo de problema seleccionado.
                cmd.Parameters.AddWithValue("@IdTipoProblema", solicitud.IdTipoProblema);

                // Prioridad seleccionada.
                cmd.Parameters.AddWithValue("@IdPrioridad", solicitud.IdPrioridad);

                // Descripción escrita por el usuario.
                cmd.Parameters.AddWithValue("@Descripcion", solicitud.Descripcion);

                con.Open();

                // Antes: el método podía no devolver nada.
                // Ahora: devuelve true si realmente insertó una fila.
                int filas = cmd.ExecuteNonQuery();

                return filas > 0;
            }
        }

        #endregion

        #region Actualizar estado

        // Cambia solamente el estado de una solicitud.
        public bool ActualizarEstado(int id, int idEstado)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Antes: se podía confundir la columna con IdEstado.
                // Ahora: se usa el nombre real de la base: EstadoSolicitudId.
                string query = @"
                    UPDATE SolicitudesSoporte
                    SET EstadoSolicitudId = @IdEstado
                    WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                // Nuevo estado seleccionado.
                cmd.Parameters.AddWithValue("@IdEstado", idEstado);

                // Solicitud que se quiere modificar.
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                // Devuelve true si modificó una fila.
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion

        #region Eliminar solicitud

        // Elimina una solicitud por Id.
        public bool Eliminar(int id)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Antes: eliminar sin WHERE sería peligroso.
                // Ahora: se elimina solo la solicitud que tenga el Id recibido.
                string query = "DELETE FROM SolicitudesSoporte WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                // Id de la solicitud que se eliminará.
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                // Devuelve true si eliminó una fila.
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion

        #region Mapeo de datos

        // Convierte una fila de SQL en un objeto SolicitudSoporte.
        private SolicitudSoporte MapearSolicitud(SqlDataReader reader)
        {
            // Antes: este bloque se repetía en ObtenerTodas y ObtenerPorId.
            // Ahora: está centralizado aquí para mantener el código más ordenado.
            return new SolicitudSoporte
            {
                // Datos principales.
                Id = Convert.ToInt32(reader["Id"]),
                UsuarioId = Convert.ToInt32(reader["UsuarioId"]),

                // Datos del usuario.
                NombreUsuario = reader["NombreUsuario"].ToString(),
                CorreoUsuario = reader["CorreoUsuario"].ToString(),

                // Datos del área.
                IdArea = Convert.ToInt32(reader["IdArea"]),
                NombreArea = reader["NombreArea"].ToString(),

                // Datos del tipo de problema.
                IdTipoProblema = Convert.ToInt32(reader["IdTipoProblema"]),
                DescripcionProblema = reader["DescripcionProblema"].ToString(),

                // Datos de prioridad.
                IdPrioridad = Convert.ToInt32(reader["IdPrioridad"]),
                NivelPrioridad = reader["NivelPrioridad"].ToString(),

                // Datos de estado.
                IdEstado = Convert.ToInt32(reader["IdEstado"]),
                NombreEstado = reader["NombreEstado"].ToString(),

                // Descripción y fecha.
                Descripcion = reader["Descripcion"].ToString(),
                FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
            };
        }

        #endregion
    }
}