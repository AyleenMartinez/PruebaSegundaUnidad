namespace PruebaSegundaUnidad.Models
{
    // Representa un tipo de problema que el usuario puede reportar.
    // Ejemplo: problema con computador, internet, software o impresora.
    public class TipoProblema
    {
        // Id del tipo de problema en la base de datos.
        public int Id { get; set; }

        // Nombre descriptivo del tipo de problema.
        public string NombreTipoProblema { get; set; }
    }
}