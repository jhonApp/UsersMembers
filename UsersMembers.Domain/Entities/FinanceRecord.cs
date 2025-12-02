namespace UsersMembers.Domain.Entities
{
    public class FinanceRecord
    {
        public string? Month { get; set; }
        public double Tithe { get; set; }
        public double Offering { get; set; }
        public string? PaymentMethod { get; set; }
        public int TransactionId { get; set; }
    }
}
