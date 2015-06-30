using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace Pentax_K_S2_Remote
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }
        private void llWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // Change the color of the link text by setting LinkVisited 
                // to true.
                llWeb.LinkVisited = true;
                //Call the Process.Start method to open the default browser 
                //with a URL:
                System.Diagnostics.Process.Start(llWeb.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Konnte Webseite nicht öffnen. "+ex.Message);
            }
        }

        private void FormAbout_Shown(object sender, EventArgs e)
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo vi = FileVersionInfo.GetVersionInfo(assembly.Location);

            lProduktname.Text = vi.ProductName;
            lProduktversion.Text = vi.ProductVersion;
            lVersion.Text = vi.FileVersion;
            lBuild.Text = vi.FileBuildPart.ToString();
            lDescription.Text = vi.FileDescription;
            lCopyright.Text = vi.LegalCopyright;


            lDotNetVersion.Text = Environment.Version.ToString();
        }
    }
}
