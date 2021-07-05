using Core.Entities;

namespace Core.Specifications
{
  public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
  {
   // конструктор без параметров
    // || или иначе, x.Name.ToLower (). Contains (productParams.Search)), чтобы найти продукты по запросу
    // Сначала мы проверяем, существует ли значение
    public ProductsWithTypesAndBrandsSpecification(ProductSpecParams productParams)
      : base(x =>
        (string.IsNullOrEmpty(productParams.Search) || x.Name.ToLower().Contains(productParams.Search)) &&
        (!productParams.BrandId.HasValue || x.ProductBrandId == productParams.BrandId) &&
        // Если false, то будет выполнена правая часть
        (!productParams.TypeId.HasValue || x.ProductTypeId == productParams.TypeId)
      )
    {
      AddInclude(x => x.ProductType);
      AddInclude(x => x.ProductBrand);
      AddInclude(x => x.Photos);
      // default poriadok po umolchaniu
      AddOrderBy(x => x.Name);
      ApplyPaging(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

      if (!string.IsNullOrEmpty(productParams.Sort))
      {
        switch (productParams.Sort)
        {
          case "priceAsc":
            AddOrderBy(p => p.Price);
            break;
          case "priceDesc":
            AddOrderByDescending(p => p.Price);
            break;
          default:
            AddOrderBy(x => x.Name);
            break;
        }
      }
    }

    // First part is the paramater for our criteria, section part is the expression
    // The base creates a new instance of our BaseSpecification
    public ProductsWithTypesAndBrandsSpecification(int id) : base(x => x.Id == id)
    {
      AddInclude(x => x.ProductType);
      AddInclude(x => x.ProductBrand);
      AddInclude(x => x.Photos);
    }

    // the other constructor (with a spec paramater)
  }
}