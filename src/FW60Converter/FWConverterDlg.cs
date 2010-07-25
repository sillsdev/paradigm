using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SIL.WordWorks.GAFAWS.FW60Converter
{
	public partial class FWConverterDlg : Form
	{
		public FWConverterDlg()
		{
			InitializeComponent();
			AccessibleName = GetType().Name;

			m_source.SeekThroughDomains();
			m_source.SelectedItemChanged += m_source_SelectedItemChanged;
		}

		public string CatInfo { get; private set; }

		public bool IncludeSubcategories
		{
			get { return m_cbIncludeSubcategories.Enabled && m_cbIncludeSubcategories.Checked; }
		}

		void m_source_SelectedItemChanged(object sender, TreeViewEventArgs e)
		{
			var selNode = e.Node;
			var tag = e.Node.Tag as string;
			m_tvPOS.Nodes.Clear();
			CatInfo = null;
			m_btnOk.Enabled = false;
			if (!tag.StartsWith("SQL"))
				return;

			SqlConnection con = null;
			try
			{
				var parts = tag.Split('^');
				var bldr = new SqlConnectionStringBuilder
							{
								DataSource = parts[1],
								InitialCatalog = parts[2],
								Password = "inscrutable",
								UserID = "sa"
							};
				var conStr = bldr.ToString();
				con = new SqlConnection(conStr);
				con.Open();
				using (var cmd = con.CreateCommand())
				{
					cmd.CommandText = "DECLARE @catListId INT\n" +
									  "SELECT @catListId=catList.Id\n" +
									  "FROM CmObject lp\n" +
									  "JOIN CmObject catList ON catList.Class$ = 8 AND catList.Owner$ = lp.Id AND catList.OwnFlid$ = 6001005\n" +
									  "WHERE lp.Class$ = 6001\n" +
									  "SELECT catId.Id, catId.Owner$, catId.OwnOrd$, catId.Level, catName.Ws, catName.Txt\n" +
									  "FROM fnGetOwnedIds(@catListId, 8008, 7004) catId\n" +
									  "JOIN CmPossibility_Name catName ON catName.Obj = catId.Id\n" +
									  "ORDER BY Level, OwnOrd$";
					cmd.CommandType = CommandType.Text;
					using (var reader = cmd.ExecuteReader())
					{
						var nodes = new Dictionary<int, TreeNode>();
						var prevCatId = 0;
						m_tvPOS.BeginUpdate();
						while (reader.Read())
						{
							var catId = reader.GetInt32(0);
							var catOwnerId = reader.GetInt32(1);
							var catOwnOrd = reader.GetInt32(2);
							var catLevel = reader.GetInt32(3);
							var catWs = reader.GetInt32(4);
							var catName = reader.GetString(5);

							if (prevCatId == catId) continue;

							var newNode = new TreeNode(catName) {Tag = catId + "^" + conStr};
							var tnCol = nodes.ContainsKey(catOwnerId) ? nodes[catOwnerId].Nodes : m_tvPOS.Nodes;
							tnCol.Add(newNode);
							nodes.Add(catId, newNode);
							prevCatId = catId;
						}
						m_tvPOS.EndUpdate();
					}
				}
			}
			finally
			{
				if (con != null)
					con.Close();
			}
		}

		private void m_tvPOS_AfterSelect(object sender, TreeViewEventArgs e)
		{
			CatInfo = (string)e.Node.Tag;
			m_btnOk.Enabled = (CatInfo != null);
			m_cbIncludeSubcategories.Enabled = e.Node.Nodes.Count > 0;
		}
	}
}