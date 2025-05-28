using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Enities;
using ProductApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Infrastructure.Repositories
{
    internal class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product enity)
        {
            try
            {
                var getProduct = await GetByAsync(_ => _.Name!.Equals(enity.Name));
                if(getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                {
                    return new Response(false, $"{enity.Name} already added");
                }

                var currentEnity = context.Products.Add(enity).Entity;
                await context.SaveChangesAsync();
                if (currentEnity is not null && currentEnity.Id > 0)
                {
                    return new Response(true, $"{enity.Name} added successfully");
                }
                else return new Response(false, $"Error occured while adding {enity.Name}");
            }
            catch(Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "An error occurred while creating the product.");
            }
        }
        
        public async Task<Response> DeleteAsync(Product enity)
        {
            try
            {
                var product = await FindByIdAsync(enity.Id);
                if (product is null)
                {
                    return new Response(false, $"{enity.Name} is not found");
                }
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return new Response(true, $"{enity.Name} is deleted successfully");
            }
            catch(Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "An error occurred while deleting the product.");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null!;
            }catch(Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception( "An error occurred while finding the product.");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;
            }
            catch(Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error occured retrieving products.");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null!;
            }
            catch(Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error occured retrieving products.");
            }
        }

        public async Task<Response> UpdateAsync(Product enity)
        {
            try
            {
                var product = await FindByIdAsync(enity.Id);
                if (product is null)
                {
                    return new Response(false, $"{enity.Name} is not found");
                }
                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(enity);
                await context.SaveChangesAsync();
                return new Response(true, $"{enity.Name} is updated");
            }
            catch(Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false,"Error occured updating products.");
            }
        }
    }
}
