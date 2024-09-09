using System;
using System.Threading.Tasks;
using DiscountGrpc.Data;
using DiscountGrpc.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DiscountGrpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly DiscountContext _context;
        public DiscountService(DiscountContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));
            }
            return coupon.ToProto();
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = new Coupon
            {
                ProductName = request.ProductName,
                Description = request.Description,
                Amount = request.Amount
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return coupon.ToProto();
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _context.Coupons.FindAsync(request.Id);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ID={request.Id} is not found."));
            }
            coupon.ProductName = request.ProductName;
            coupon.Description = request.Description;
            coupon.Amount = request.Amount;
            
            await _context.SaveChangesAsync();

            return coupon.ToProto();
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _context.Coupons.FindAsync(request.Id);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ID={request.Id} is not found."));
            }
            _context.Coupons.Remove(coupon);
            var deleteResult = await _context.SaveChangesAsync();

            var response = new DeleteDiscountResponse
            {
                Success = deleteResult > 0,
                Message = deleteResult > 0 ? $"Discount with ID={request.Id} is successfully deleted" : $"Failed to delete discount with ID={request.Id}"
            };

            return response;
        }
    }
}