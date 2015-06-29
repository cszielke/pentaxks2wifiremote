using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pentax_K_S2_Remote
{
    public partial class FormOptions : Form
    {
        public string IPAddrCamera 
        {
            get 
            {
                return tbIPAddrCamera.Text;
            } 
            set
            {
                tbIPAddrCamera.Text = value;
            }
        }

        public bool SimulateCamera
        {
            get
            {
                return cbSimCam.Checked;
            }
            set
            {
                cbSimCam.Checked = value;
            }
        }

        public FormOptions()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
