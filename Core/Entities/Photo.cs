namespace Core.Entities
{

    // Здесь мы собираемся «полностью определить» связь между фотографией и товаром - это будет означать, что при удалении товара,
    // тогда это также перейдет к фотографии и также удалит это.
    public class Photo : BaseEntity
    {
        public string PictureUrl { get; set; }
        public string FileName { get; set; }
        public bool IsMain { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set;}
        
    }
}