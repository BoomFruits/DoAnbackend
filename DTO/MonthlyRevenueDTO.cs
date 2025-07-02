namespace DoAn.DTO
{
    public class MonthlyRevenueDTO
    {
        public string Month { get; set; } = string.Empty;
        public decimal Current { get; set; }    // Doanh thu tháng này
        public decimal Previous { get; set; }   // So với năm trước hoặc giả lập
        public decimal BEP { get; set; }        // Điểm hòa vốn giả lập
    }
}
