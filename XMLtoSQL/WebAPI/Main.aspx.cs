using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAPI
{
    public partial class Main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void SendToNewPage(object sender, EventArgs e)
        {
            var regNumber = (byte)0;
            if (!byte.TryParse(tbRegionName.Text, out regNumber) && tbRegionName.Text != string.Empty)
            {
                this.tbRegionName.Text = "Неверно";
            }
            else
            {
                Response.Redirect($"Table.aspx?reg={regNumber.ToString()}&offRay={this.chbRayonOff.Checked}&ray={this.tbRayonName.Text}&offTwn={this.chbTownOff.Checked}&twn={this.tbTownName.Text}&offStr={this.chbStreetOff.Checked}&str={this.tbStreetName.Text}&useHouse={this.chbUseHouse.Checked}&houseNum={this.tbHouseNumber.Text}&postal={this.tbPostalIndex.Text}");
            }
        }
    }
}