namespace OfferVerse.Models
{
    public class Category
    {
        private int categoryId;
        private string name;

        public int CategoryId { get { return categoryId; } set { categoryId = value; } }
        public string Name { get { return name; } set { name = value; } }

        public Category() { }

        public Category(int categoryId, string name)
        {
            CategoryId = categoryId;
            Name = name;
        }
    }
}
