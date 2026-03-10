namespace BlazorApp1.Client.Models
{
    public class TableRow
    {
        public string Dato1 { get; set; } = "";
        public string Dato2 { get; set; } = "";
        public string Dato3 { get; set; } = "";
        public string Dato4 { get; set; } = "";
        public string Dato5 { get; set; } = "";
        public string Dato6 { get; set; } = "";
        public string Dato7 { get; set; } = "";

        // Campos para tipos específicos
        public bool Activo { get; set; }
        public bool EsPropietario { get; set; }
        public bool PagoRequerido { get; set; }
        public decimal Valor { get; set; }
        public decimal Valor2 { get; set; }
        public DateTime? Fecha1 { get; set; }
        public DateTime? Fecha2 { get; set; }
        public DateTime? Hora1 { get; set; }
        public DateTime? Hora2 { get; set; }
    }
}
