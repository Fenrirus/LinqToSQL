using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToSQL
{
    public class DwieKolumny
    {
        public string Kolor;
        public string Styl;
        public DwieKolumny(string styl, string kolor)
        {
            Kolor = kolor;
            Styl = styl;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TSQLV4DataContext dc = new TSQLV4DataContext();
            Console.WriteLine("Co chcesz wykonać?");
            Console.WriteLine("1 : zwracanie danych z tabeli Product");
            Console.WriteLine("2 : cena większa od 300");
            Console.WriteLine("3 : Własny typ");
            Console.WriteLine("4 : Anonimowy typ");
            Console.WriteLine("5 : Dane z dwóch tabel");
            Console.WriteLine("6 : Dane z dwóch polączonych tabel");
            Console.WriteLine("7 : Dane z dwóch polączonych tabel (entityRef)");
            Console.WriteLine("8 : Update");
            Console.WriteLine("9 : INSERT");
            Console.WriteLine("10 : DELETE");
            int liczba = Convert.ToInt32(Console.ReadLine());
            switch (liczba)
            {
                case 1:
                    daneZTabeli(dc);
                    break;
                case 2:
                    daneZTabeliWhere(dc);
                    break;
                case 3:
                    daneZTabeliDwieKolumntWłasnyTyp(dc);
                    break;
                case 4:
                    daneZTabeliNieokreślonyTyp(dc);
                    break;
                case 5:
                    przeszukiwanieDwochTabel(dc);
                    break;
                case 6:
                    przeszukiwanieDwochPolaczonychTabel(dc);
                    break;
                case 7:
                    entityRef(dc);
                    break;
                case 8:
                    UPDATE(dc);
                    break;
                case 9:
                    INSERT(dc);
                    break;
                case 10:
                    DELETE(dc);
                    break;
                default:
                    break;
            }
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
        static void daneZTabeliWhere(TSQLV4DataContext dc)
        {
            var data = from p in dc.Product
                       where p.ListPrice > 300
                       select p;
            foreach (var item in data)
            {
                Console.WriteLine("Id: {0}, Nazwa: {1}, Cena: {2}", item.ProductID, item.Name, item.ListPrice);
            }
            Console.ReadKey();
        }
        static void daneZTabeliDwieKolumntWłasnyTyp(TSQLV4DataContext dc)
        {
            var data = from p in dc.Product
                       where p.ListPrice > 250
                       select new DwieKolumny(p.Style, p.Color);
            foreach(DwieKolumny item in data)
            {
                Console.WriteLine($"Styl: {item.Styl}, Kolor: {item.Kolor}");
            }
            Console.ReadKey();
        }
        static void daneZTabeliNieokreślonyTyp(TSQLV4DataContext dc)
        {
            var data = from p in dc.Product
                       where p.ListPrice > 500
                       select new { nazwa_produktu = p.Name, linia_produktu = p.ProductLine };
            foreach (var item in data)
            {
                Console.WriteLine($"Nazwa produktu: {item.nazwa_produktu}, Linia produktu: {item.linia_produktu}");
            }
            Console.ReadKey();
        }
        static void przeszukiwanieDwochTabel(TSQLV4DataContext dc)
        {
            int ilosc = 0;
            var data = from p in dc.Product
                       from pc in dc.ProductCategory
                       where p.ListPrice > 300 && pc.Name == "Clothing"
                       select new { nazwa_produktu = p.Name, nazwa_kategorii = pc.Name };
            foreach (var item in data)
            {
                ilosc++;
                Console.WriteLine($"Nazwa produktu: {item.nazwa_produktu}, Nazwa kategorii: {item.nazwa_kategorii} policz ilosc:{ilosc}");
            }
            Console.ReadKey();
        }
        static void przeszukiwanieDwochPolaczonychTabel(TSQLV4DataContext dc)
        {
            int ilosc = 0;
            var data = from p in dc.Product
                       from psc in dc.ProductSubcategory
                       where p.ProductSubcategoryID == psc.ProductSubcategoryID
                       select new {p.ProductID, p.ProductSubcategoryID, psc.Name };
            foreach (var item in data)
            {
                ilosc++;
                Console.WriteLine("Id: {0}, Id Subkategorii: {1}, Nazwa: {2}, ilosc {3}", item.ProductID, item.ProductSubcategoryID, item.Name, ilosc);
            }
            Console.ReadKey();
        }
        static void entityRef (TSQLV4DataContext dc)
        {
            var data = from p in dc.Product
                       select new
                       {
                           SubcategoryName = p.ProductSubcategory.Name,
                           ProductName = p.Name,
                           ProducktId = p.ProductID
                       };
            foreach(var item in data)
            {
                Console.WriteLine($"Nazwa produktu {item.ProductName}, Id produktu {item.ProducktId}, Nazwa podkategorii {item.SubcategoryName}");
            }
            Console.ReadKey();
        }
        static void UPDATE(TSQLV4DataContext dc)
        {
            var update = from p in dc.Product
                         where p.Name.Contains("Tube")
                         select p;
            int i = 0;
            foreach(var item in update)
            {
                item.Name = "Tuuube" + i.ToString();
                i++;
            }
            dc.SubmitChanges();
            var poUpdate = from p in dc.Product
                         where p.Name.Contains("Tuuube")
                         select p;
            foreach(var item in poUpdate)
            {
                Console.WriteLine("Id: {0}, Nazwa: {1}", item.ProductID, item.Name);
            }
            Console.ReadKey();
        }
        static void INSERT(TSQLV4DataContext dc)
        {
            ProductCategory prod = new ProductCategory();
            prod.Name = "Test";
            prod.ModifiedDate = System.DateTime.Now;
            dc.ProductCategory.InsertOnSubmit(prod);
            dc.SubmitChanges();
            var lastrow = (from p in dc.ProductCategory
                           orderby p.ProductCategoryID descending
                           select p).First();
            Console.WriteLine("Id: {0}, Nazwa: {1}", lastrow.ProductCategoryID, lastrow.Name);
            Console.ReadKey();
        }
        static void DELETE(TSQLV4DataContext dc)
        {
            var delete = from p in dc.ProductCategory
                           where p.Name.Contains("Test")
                           select p;
            dc.ProductCategory.DeleteAllOnSubmit(delete);
            dc.SubmitChanges();
            var afterChanges = from p in dc.ProductCategory
                               select p;
            foreach (var item in afterChanges)
            {
                Console.WriteLine("Id: {0}, Nazwa: {1}", item.ProductCategoryID, item.Name);
            }
            Console.ReadKey();
        }
    }
}
