using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TryGetRederResult
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var connection = new SqlConnection();
            connection.ConnectionString = @"Data Source=DESKTOP-17UB0HN\SQLEXPRESS;Initial Catalog=RestrictedFIAS;Persist Security Info=True;User ID=sa;Password=superadmin;Context Connection=False";
            connection.Open();
            var mainQuerry = @"SELECT [Region].[REGIONCODE] AS RegionCode, [Region].[OFFNAME] AS RegionName, [Rayon].[OFFNAME] AS RayonName, [Town].[OFFNAME] AS TownName, [Street].[OFFNAME] AS StreetName FROM [Region] INNER JOIN Rayon INNER JOIN Town INNER JOIN Street ON [Street].[PARENTGUID] = [Town].[AOGUID] ON [Town].[PARENTGUID] = [Rayon].[AOGUID] ON [Rayon].[PARENTGUID] = [Region].[AOGUID] WHERE ([Region].[REGIONCODE] = 64)";
            var command = new SqlCommand(mainQuerry, connection);
            var result = command.ExecuteReader();
            result.Read();
            this.label1.Text = result[4].ToString();
        }
    }
}
