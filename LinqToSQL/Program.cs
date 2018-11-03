using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            TSQLV4DataContext dc = new TSQLV4DataContext();
            daneZTabeli(dc);
        }
        static void daneZTabeli(TSQLV4DataContext dc)
        {
            var data = from p in dc.Product
                       select p;
            var firstFive = data.Take(5);
            foreach (var item in firstFive)
            {
                Console.WriteLine("Id: {0}, Numer: {1}, Nazwa: {2}", item.ProductID, item.ProductNumber, item.Name);
            }
            Console.ReadKey();
        }
    }
}
