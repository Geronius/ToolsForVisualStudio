using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.Administration;

namespace DeploymentFrameworkForBizTalk.Addin.Implementation
{
    public partial class BiztalkHostsForm : Form
    {

        public bool BounceHost {get{ return Properties.Settings.Default.BounceHost;}}
        private string CurrentHost { get { return Properties.Settings.Default.CurrentHost; } set { Properties.Settings.Default.CurrentHost = value; } }

        public ManagementObject CheckedHost { get { return ((KeyValuePair<string, ManagementObject>)lstHosts.CheckedItems[0]).Value; } }
        public BiztalkHostsForm()
        {
            TopMost = true;
            TopMost = false;


            InitializeComponent();


            lstHosts.ItemCheck += SetCheck;
            //checkedListBox1.bin += LstHostsItemBinding;

            LoadList();
            SetButtonStatus();
        }


        //private static void LstHostsItemBinding(object sender, CheckBoxList.CheckBoxList.ItemBindingEventArgs e)
        //{
        //    if (!((bool)((KeyValuePair<string, ManagementObject>)(e.Item.DataItem)).Value.Properties["IsDisabled"].Value)) return;
        //    e.Item.Enabled = false;
        //    e.Item.Text += @" (Disabled)";
        //}

        private void SetCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                for (int i = 0; i < lstHosts.Items.Count; i++)
                {
                    if (i != e.Index)
                        lstHosts.SetItemChecked(i, false);
                }
                CurrentHost = ((KeyValuePair<string, ManagementObject>)lstHosts.Items[e.Index]).Key;
            }
            if (IsHandleCreated)
            {
                this.BeginInvoke(new Action(() =>
             {
                 SetButtonStatus();
             }));
            }
            //if (!(bool)((KeyValuePair<string, ManagementObject>)(e.Item.DataItem)).Value.Properties["IsDisabled"].Value)
            //    return;

            //e.Cancel = true;
        }

        private void SetButtonStatus()
        {
            if (lstHosts.CheckedItems.Count == 0)
            {
                btnAttach.Enabled = false;
            }
            else
            {
                btnAttach.Enabled = true;
            }

        }

        private void LoadList()
        {
            var hosts = GetBiztalkHosts();


            //foreach (var prop in hosts.FirstOrDefault(n=> n.Key.Contains("ProcessingHost")).Value?.Properties)
            //{

            //    Console.WriteLine(prop.Name + ":");
            //    Console.WriteLine(prop.Value);

            //}



            lstHosts.DataSource = new BindingSource(hosts, null);
            lstHosts.DisplayMember = "Key";
            lstHosts.ValueMember = "Value";

            for (int i = 0; i < lstHosts.Items.Count; i++)
            {
                if (CurrentHost == ((KeyValuePair<string, ManagementObject>)lstHosts.Items[i]).Key)
                    lstHosts.SetItemChecked(i, true);
            }
            
        }

        public Dictionary<string, ManagementObject> GetBiztalkHosts()
        {
            //set up a WMI query to acquire a list of orchestrations with the given Name and 
            //AssemblyName key values.  This should be a list of zero or one Orchestrations.
            //Create the WMI search object.
            var searcher = new ManagementObjectSearcher();

            // create the scope node so we can set the WMI root node correctly.
            var scope = new ManagementScope("root\\MicrosoftBizTalkServer");
            searcher.Scope = scope;

            var query = new SelectQuery { QueryString = "Select * from MSBTS_HostInstance where HostType=1 and ServiceState = 4" };

            // Set the query for the searcher.
            searcher.Query = query;
            var queryCol = searcher.Get();

            return queryCol.Cast<ManagementObject>().ToDictionary(result => (string)result.Properties["HostName"].Value);
        }

        //private void BtnExecuteClick(object sender, EventArgs e)
        //{
        //    //if (!lstHosts.CheckedItems.Any() && !cbRecycleAppPools.Checked)
        //    //    return;
        //    lblStatus.Text = "";

        //    TopMost = false;
        //    Enabled = false;
        //    try
        //    {
        //        var lst = lstHosts.CheckedItems;

        //        lblStatus.Text = "";
        //        lblStatus.Visible = true;

        //        var cancelToken = new CancellationTokenSource();
        //        Task.WaitAll(lst.Select(item => ((KeyValuePair<string, ManagementObject>)(item.DataItem)).Value).Select(i1 => Task.Factory.StartNew(() =>
        //        {
        //            BounceHost(i1);
        //        }, cancelToken.Token)).ToArray());
        //        cancelToken.Cancel();

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    finally
        //    {
        //        lblStatus.Visible = false;
        //        TopMost = false;
        //    }
        //    Close();
        //}


        //private void BounceHost(ManagementObject service)
        //{
        //    // 1 Stopped
        //    // 2 start pending
        //    // 3 Stop pending
        //    // 4 Started
        //    try
        //    {
        //        if (cbStopHosts.Checked)
        //        {
        //            service.InvokeMethod("Stop", null);
        //        }

        //        Thread.Sleep(200);

        //        if (cbStartHosts.Checked)
        //        {

        //            service.InvokeMethod("Start", null);
        //        }
        //    }
        //    catch
        //    {
        //        // ignored
        //    }
        //}

        private void BiztalkHostsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
        }
    }
}