namespace SemesterWork
{
    public class DBProductData
    {
        public ProductData Data { get; set; }
        public bool IsInDB { get; set; }

        public DBProductData(ProductData data, bool isInDB)
        {
            Data = data;
            IsInDB = isInDB;
        }
    }
}
