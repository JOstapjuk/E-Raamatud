using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Raamatud.Model
{
    class PurchaseBasket
    {
        [PrimaryKey, AutoIncrement]
        public int Ostukorv_ID { get; set; }

        public int Kasutaja_ID { get; set; }

        public int Raamat_ID { get; set; }

        public int Kogus { get; set; }

        [MaxLength(10)]
        public decimal Lõppu_hind { get; set; }
    }
}
