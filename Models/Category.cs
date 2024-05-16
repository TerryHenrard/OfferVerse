using OfferVerse.DAL.Interfaces;

namespace OfferVerse.Models
{
    public class Category
    {
        private int categoryId;
        private string name;
        private string imagePath;

        public int CategoryId 
        { 
            get { return categoryId; } 
            set { categoryId = value; } 
        }
        public string Name 
        { 
            get { return name; } 
            set { name = value; } 
        }
        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }

        //Constructors
        public Category() { }

        public Category(int categoryId)
        {
            CategoryId = categoryId;
        }

        public Category(int categoryId, string name, string imagePath)
        {
            CategoryId = categoryId;
            Name = name;
            ImagePath = imagePath;
        }

        //Static methods
        public static List<Category> GetCategories(ICategoryDAL dal)
        {
            return dal.GetCategories();
        }

        //Overrided Methods
        public override string ToString() => $" , {CategoryId}, {Name}";

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if(obj == null)
            {
                return false;
            }

            return ToString() == obj.ToString();
        }
    }
}