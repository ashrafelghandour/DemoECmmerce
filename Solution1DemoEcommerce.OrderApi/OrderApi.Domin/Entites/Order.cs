using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Domin.Entites
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ClientId { get; set; }

        public  int PurchaseQuntity { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
       

    }
}
