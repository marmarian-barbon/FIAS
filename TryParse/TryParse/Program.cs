using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TryParse
{
    class Program
    {
        static void Main(string[] args)
        {
            var doc = new XmlDocument();
            var path1 = @"F:\Browser\fias_xml\AS_CENTERST_20171214_aaebe8b4-4476-4772-8f54-5a017b471cde.XML";
            var path2 = @"F:\Browser\fias_xml\AS_HOUSE_20171214_ad9f8dea-850f-4f70-9f54-1e05d9ecde76.XML";
            var path3 = @"F:\Browser\fias_xml\AS_ADDROBJ_20171214_4af453b1-c874-4a55-935d-91aa70546984.XML";
            var path4 = @"F:\Browser\fias_xml\AS_SOCRBASE_20171214_f95c3d3c-fbbd-4dc6-99e5-a35fb4505b82.XML";

            var connectionString = new SqlConnectionStringBuilder
            {
                Authentication = SqlAuthenticationMethod.SqlPassword,
                DataSource = @".\SQLEXPRESS",
                InitialCatalog = "RestrictedFIAS",
                UserID = "sa",
                Password = "superadmin",
                TrustServerCertificate = true
            };

            var connection = new SqlConnection(connectionString.ConnectionString);
            connection.Open();
            Console.WriteLine(connection.State);
            connection.Close();
            var currentDate = DateTime.Today;
            using (var reader = XmlReader.Create(path2))
            {
                while (reader.Read())
                {
                    if (reader.GetAttribute("HOUSEGUID") == "43cbf44f-3c30-4656-98ac-839f49e12dee")
                    {
                        if (DateTime.Parse(reader.GetAttribute("ENDDATE")) >= currentDate)
                        {
                            for (var i = 0; i < reader.AttributeCount; i++)
                            {
                                reader.MoveToAttribute(i);
                                Console.WriteLine("{0} = {1}", reader.Name, reader.Value);
                            }
                        }
                    }
                }
            }
        }
    }
}
