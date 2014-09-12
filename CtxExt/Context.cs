using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtxExt
{
    public class Context : DbContext
    {
        public DbSet<Person> People { get; set; }
    }
}
