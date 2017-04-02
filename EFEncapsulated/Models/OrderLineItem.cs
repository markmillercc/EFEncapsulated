namespace EFEncapsulated.Models
{
    public class OrderLineItem
    {
        public int Id { get; set; }

        public string Description { get; private set; }
        public double Price { get; private set; }

        private OrderLineItem()
        {
        }

        public OrderLineItem(string description, double price)
        {
            Description = description;
            Price = price;
        }
    }
}