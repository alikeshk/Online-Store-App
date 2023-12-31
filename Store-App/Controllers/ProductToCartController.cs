﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Store_App.Helpers;
using Store_App.Models.DBClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Store_App.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProductToCartController : ControllerBase
    {
        private readonly StoreAppDbContext _context;

        public ProductToCartController(StoreAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductToCart>>> GetProductsInCartForCurrentUser()
        {
            Person? person = UserHelper.GetCurrentUser();
            
            var productsInCart = await _context.ProductToCarts
                .Where(ptc => ptc.CartId == person.CartId)
                .Include(ptc => (ptc.Product))
                .ToListAsync();

            if (productsInCart == null)
            {
                return NotFound();
            }

            if (!productsInCart.Any())
            {
                return NotFound("Cart is Empty.");
            }
            return productsInCart;
        }

        [HttpGet("{cartId}")]
        public async Task<ActionResult<IEnumerable<ProductToCart>>> GetProductsInCart(int cartId)
        {
            var productsInCart = await _context.ProductToCarts
                .Where(ptc => ptc.CartId == cartId)
                .Include(ptc => ptc.Product)
                .ToListAsync();

            if (productsInCart == null)
            {
                return NotFound();
            }

            if (!productsInCart.Any())
            {
                return NotFound("Cart is Empty.");
            }

            return productsInCart;
        }

        [HttpPost("{cartId}/products/{productId}")]
        public async Task<ActionResult<ProductToCart>> AddProductToCart(int cartId, int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            var cart = await _context.Carts.FindAsync(cartId);

            if (product == null || cart == null)
            {
                return NotFound();

            }

            var sale = await _context.Sales.FindAsync(product.SaleId);
            bool productOnSale = sale != null && sale.StartDate < DateTime.UtcNow && sale.EndDate > DateTime.UtcNow;
            var productPrice = productOnSale ? product.Price * (1 - (double)sale.PercentOff * 0.01) : product.Price;
            cart.Total += productPrice ?? 0;

            _context.Entry(cart).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            var productToCart = new ProductToCart
            {
                CartId = cartId,
                ProductId = productId
            };

            _context.ProductToCarts.Add(productToCart);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductsInCart), new { cartId = cartId }, productToCart);
        }

        [HttpDelete("{cartId}/products/{productId}")]
        public async Task<ActionResult> RemoveProductFromCart(int cartId, int productId)
        {
            try
            {
                // Log or add breakpoints to check values
                Console.WriteLine($"Removing product {productId} from cart {cartId}");

                var product = await _context.Products.FindAsync(productId);
                var cart = await _context.Carts.FindAsync(cartId);

                if (product == null || cart == null)
                {
                    return NotFound();

                }

                var productToCart = await _context.ProductToCarts
                    .Where(ptc => ptc.CartId == cartId && ptc.ProductId == productId)
                    .ToListAsync(); // Ensure asynchronous query execution

                if (productToCart == null || !productToCart.Any())
                {
                    Console.WriteLine($"Product {productId} not found in cart {cartId}");
                    return NotFound();
                }

                _context.ProductToCarts.Remove(productToCart.First());
                await _context.SaveChangesAsync();

                Console.WriteLine($"Product {productId} removed successfully from cart {cartId}");

                var sale = await _context.Sales.FindAsync(product.SaleId);
                bool productOnSale = sale != null && sale.StartDate < DateTime.UtcNow && sale.EndDate > DateTime.UtcNow;
                var productPrice = productOnSale ? product.Price * (1 - (double)sale.PercentOff * 0.01) : product.Price;
                cart.Total -= productPrice ?? 0;

                _context.Entry(cart).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error removing product from cart: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveAllProductsFromCartForCurrentUser()
        {
            try
            {
                Person? person = UserHelper.GetCurrentUser();

                var productsInCart = await _context.ProductToCarts
                    .Where(ptc => ptc.CartId == person.CartId)
                    .Include(ptc => (ptc.Product))
                    .ToListAsync();

                if (productsInCart == null || !productsInCart.Any())
                {
                    return NotFound();
                }
                
                _context.ProductToCarts.RemoveRange(productsInCart);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error removing product from cart: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
