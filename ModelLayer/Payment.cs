using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
    public class Payment
    {
        public int pago_id { get; set; }
        public int deuda_id { get; set; }
        public decimal monto { get; set; }
        public DateTime fecha_pago { get; set; }
    }

}
