using System;
using System.ComponentModel.DataAnnotations;

namespace PawnShop.Models
{
    public class Transactions
    {
        [Key]
        public int TransactionID { get; set; }
        public int PledgeID { get; set; }
        public int EmployeeId { get; set; } // Внешний ключ для связи с Employees
        public string TransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }

        // Навигационные свойства
        public virtual Pledge Pledge { get; set; } // Связь с Pledge
        public virtual Employees Employee { get; set; } // Связь с Employees
    }
}