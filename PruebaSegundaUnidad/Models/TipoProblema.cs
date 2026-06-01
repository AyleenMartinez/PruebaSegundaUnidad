namespace PruebaSegundaUnidad.Models
{
    /// <summary>
    /// Define las categorías de los incidentes que los usuarios pueden reportar.
    /// (Ejemplo: Problema con computador, Internet, Software).
    /// </summary>
    public class TipoProblema
    {
        public int Id { get; set; }
        public string NombreTipoProblema { get; set; }
    }
}