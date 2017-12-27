using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrySortGUIDs
{
    class Program
    {
        static void Main(string[] args)
        {
            var GUIDs = new List<Guid>();
            var random = new Random();
            for (var i = 0; i < 10; i++)
            {
                var bytes = new byte[16];
                random.NextBytes(bytes);   
                GUIDs.Add(new Guid(bytes));
                Console.WriteLine(GUIDs[i].ToString());
            }

            var SortedGUIDs = new SortedSet<Guid>(GUIDs);
            Console.WriteLine(SortedGUIDs.Count);
            using(var enumerator = SortedGUIDs.GetEnumerator())
            {
                while(enumerator.MoveNext())
                {
                    Console.WriteLine(enumerator.Current.ToString());
                }
            }
        }
    }
}
