using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Social_Network
{
    class PaginatedListGeneric<TEntity>: List<TEntity>
    {
        public int PageIndex { get; private set; } 
        //public int TotalPages { get; private set; }

        public PaginatedListGeneric(List<TEntity> items, /*int count*/int pageIndex ,int pageSize)
        {
            PageIndex = pageIndex;
            //TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        //public bool HasPreviousPage
        //{
        //    get
        //    {
        //        return (PageIndex > 1);
        //    }
        //}

        //public bool HasNextPage
        //{
        //    get
        //    {
        //        return (PageIndex < TotalPages);
        //    }
        //}

        public static async Task<PaginatedListGeneric<TEntity>> CreateAsync(IQueryable<TEntity> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedListGeneric<TEntity>(items, /*count*/ pageIndex, pageSize);
        }
    }
}
