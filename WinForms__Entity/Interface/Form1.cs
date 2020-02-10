using DB_Purchases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq.Expressions;

namespace Interface
{
    public partial class Form1 : Form
    {
        Dictionary<CheckBox, DataGridViewTextBoxColumn> UIElementLinks = new Dictionary<CheckBox, DataGridViewTextBoxColumn>();
        public Form1()
        {
            InitializeComponent();
            UIElementLinks.Add(PurchaseDateChk, purchaseDateDataGridViewTextBoxColumn);
            UIElementLinks.Add(OrganizationChk, organizationDataGridViewTextBoxColumn);
            UIElementLinks.Add(CityChk, cityDataGridViewTextBoxColumn);
            UIElementLinks.Add(CountryChk, countryDataGridViewTextBoxColumn);
            UIElementLinks.Add(ManagerChk, managerDataGridViewTextBoxColumn);
            restoregriddata();

        }

        void restoregriddata()
        {
            using (Model1Container db = new Model1Container())
                purchasesBindingSource.DataSource = db.Purchases.ToList();

            button1.Enabled = false;
            foreach (var item in UIElementLinks)
            {
                item.Key.Checked = false;
                item.Value.Visible = true;
            }
        }
        private string getKey(Purchase p)
        {
            var res = string.Empty;

            if (PurchaseDateChk.Checked)
                res += p.PurchaseDate.ToShortDateString();
            if (OrganizationChk.Checked)
                res += p.Organization;
            if (CityChk.Checked)
                res += p.City;
            if (CountryChk.Checked)
                res += p.Country;
            if (ManagerChk.Checked)
                res += p.Manager;
            return res;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            foreach (var item in UIElementLinks)
            {
                item.Value.Visible = item.Key.Checked;
            }


            var res = new Dictionary<string, List<Purchase>>();
            using (Model1Container db = new Model1Container())
            {
                var x = db.Purchases.ToList();
                foreach (var item in x)
                {
                    var keystr = getKey(item);
                    if (!res.ContainsKey(keystr))
                        res.Add(keystr, new List<Purchase>());
                    res[keystr].Add(item);
                }

                var list = new List<Purchase>();
                foreach (var g in res)
                {
                    Purchase purchase = null;
                    foreach (var t in g.Value)
                    {
                        if (purchase == null)
                            purchase = t;
                        else
                        {
                            purchase.Quantity += t.Quantity;
                            purchase.Amount += t.Amount;
                        }
                    }
                    list.Add(purchase);
                }

                purchasesBindingSource.DataSource = list;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            InitializeTable.Initialize();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            restoregriddata();
        }

        private void Check_changed(object sender, EventArgs e)
        {
            button1.Enabled = false;
            foreach (var item in UIElementLinks)
            {
                if (item.Key.Checked == true)
                {
                    button1.Enabled = true;
                    break;
                }

            }
        }
    }

    static class InitializeTable
    {
        public static void Initialize()
        {
            var model = new Model1Container();
            Random rand = new Random();
            var temp = new Purchase();
            string[] cities = { "Kiev", "London", "Moscow", "Paris" };
            string[] countries = { "Ukraine", "GB", "Russia", "France" };
            string[] managers = { "Ivan", "Mark", "Ostap" };
            string[] organizations = { "Google", "Microsoft", "Oracle" };
            int[] amounts = { 10, 25, 50, 100 };
            DateTime[] dates = { new DateTime(2016, 5, 7), new DateTime(2017, 6, 8), new DateTime(2020, 4, 18) };


            model.Purchases.RemoveRange(model.Purchases);
            for (int i = 0; i < 10; i++)
            {
                temp.PurchaseDate = dates[rand.Next(0, 3)];
                temp.City = cities[rand.Next(0, 4)];
                temp.Organization = organizations[rand.Next(0, 3)];
                temp.Country = countries[rand.Next(0, 4)];
                temp.Manager = managers[rand.Next(0, 3)];
                temp.Quantity = rand.Next(0, 10);
                temp.Amount = temp.Quantity * amounts[rand.Next(0, 4)];

                model.Purchases.Add(temp);
                model.SaveChanges();
            }
        }
    }
}
