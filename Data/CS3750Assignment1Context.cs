using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Data
{
    public class CS3750Assignment1Context : DbContext
    {
        public CS3750Assignment1Context (DbContextOptions<CS3750Assignment1Context> options)
            : base(options)
        {
        }

        public DbSet<CS3750Assignment1.Models.Account> Account { get; set; } = default!;
    }
}
