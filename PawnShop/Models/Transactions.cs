using System;
using System.ComponentModel.DataAnnotations;

namespace PawnShop.Models
{
    public class Transactions
    {
        [Key]
        public int TransactionID { get; set; }
        public int PledgeID { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }

        public virtual Pledge Pledge { get; set; } // Навигационное свойство
    }
}
