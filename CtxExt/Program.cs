using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtxExt
{
    class Program
    {
        static void Main(string[] args)
        {
            
            using (var ctx = new Context())
            {
                ctx.SAVE<Context, Person>(new Person() { Id = 1, Name = "This was changes" });
                ctx.SAVE<Context, Person>(new Person() { Name = "asd" });
                var result = ctx.ALL<Context, Person>().ToList();
                Console.WriteLine("Количество записей {0}", result.Count);
                for (int i = 0; i < result.Count; i++)
                {
                    Console.WriteLine(result[i].Name);
                }
                var last = ctx.GET<Context, Person>(2);
                Console.WriteLine("First name is "+last.Name);
            }
            Console.ReadLine();
        }
    }
}
