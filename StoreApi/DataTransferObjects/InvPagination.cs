using StoreApi.Models;

namespace StoreApi.DataTransferObjects
{
    public class InvPagination
    {
        public List<Inventory> inventories;
        public int page;
        public int count;
    }
}
