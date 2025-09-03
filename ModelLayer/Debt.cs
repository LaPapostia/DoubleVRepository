using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
    class Debt
    {
        public int deuda_id { get; set; }
        public int deudor_id { get; set; }
        public int acreedor_id { get; set; }
        public decimal monto { get; set; }
        public decimal saldo { get; set; }
        public string estado { get; set; } = "Pendiente";
        public DateTime fecha_creacion { get; set; }
    }
}








