using StoreApi.Models;

namespace StoreApi.DataTransferObjects
{
    public class ProdInvPagination
    {
        public IQueryable<Product> products;
        public int page;
        public int count;
    }
}
