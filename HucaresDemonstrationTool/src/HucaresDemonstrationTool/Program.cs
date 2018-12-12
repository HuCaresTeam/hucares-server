using SqliteManipulation;
using SqliteManipulation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HucaresDemonstrationTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var sqlite = new SqliteDataGetter();

            var dlpList = sqlite.GetData<DLP>("SELECT * FROM DLP");
            var mlpList = sqlite.GetData<MLP>("SELECT * FROM MLP");
            var cameraList = sqlite.GetData<Camera>("SELECT * FROM Camera");

            DumpEnumerable(dlpList);
            DumpEnumerable(mlpList);
            DumpEnumerable(cameraList);

            Console.ReadKey();
        }

        static void DumpEnumerable<T>(IEnumerable<T> enumerable)
        {
            var itemType = typeof(T);
            Console.WriteLine($"Dump of {itemType.Name}");
            foreach (T item in enumerable)
            {
                DumpItem(item);
            }
        }

        static void DumpItem<T>(T item)
        {
            var itemType = typeof(T);
            var properties = itemType.GetProperties();
            foreach (var propertyInfo in properties)
            {
                Console.WriteLine($"\t{propertyInfo.Name} - {propertyInfo.GetValue(item)}");
            }
            Console.WriteLine();
        }
    }
}