
namespace DiscountGrpc.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Amount { get; set; }

        public static Coupon FromProto(CouponModel proto)
        {
            return new Coupon
            {
                Id = proto.Id,
                ProductName = proto.ProductName,
                Description = proto.Description,
                Amount = (int)proto.Amount
            };
        }

        public CouponModel ToProto()
        {
            return new CouponModel
            {
                Id = Id,
                ProductName = ProductName,
                Description = Description,
                Amount = Amount
            };
        }
    }
}