using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using log4net;
using log4net.Core;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System.Net;
using PtxK_S2;
using System.Threading;

namespace Pentax_K_S2_Remote
{
    public partial class FormMain : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private PtxK_S2.K_S2 ks2;

        private bool propsset=false;
        private Cursor oldcur;
        private MJPEGSource mjsource;
        private Thumbnails thumbnailtread;

        #region Form functions
        /// <summary>
        /// Initialisiert das Logging und die Debug TextBox
        /// </summary>
        private void InitLog()
        {

            #region RichTextBoxAppender
            RichTextBoxAppender rba = new RichTextBoxAppender(rtbDebug);
            rba.Threshold = Level.All;
            rba.Layout = new log4net.Layout.PatternLayout("%date{dd-MM-yyyy HH:mm:ss.fff} %5level %message %n");

            LevelTextStyle ilts = new LevelTextStyle();
            ilts.Level = Level.Info;
            ilts.TextColor = Color.DarkBlue;
            ilts.PointSize = 8.0f;
            rba.AddMapping(ilts);

            LevelTextStyle dlts = new LevelTextStyle();
            dlts.Level = Level.Debug;
            dlts.TextColor = Color.DeepSkyBlue;
            dlts.PointSize = 8.0f;
            rba.AddMapping(dlts);

            LevelTextStyle wlts = new LevelTextStyle();
            wlts.Level = Level.Warn;
            wlts.TextColor = Color.Gold;
            wlts.PointSize = 8.0f;
            rba.AddMapping(wlts);

            LevelTextStyle elts = new LevelTextStyle();
            elts.Level = Level.Error;
            elts.TextColor = Color.Crimson;
            elts.BackColor = Color.Cornsilk;
            elts.PointSize = 8.0f;
            rba.AddMapping(elts);

            LevelTextStyle flts = new LevelTextStyle();
            flts.Level = Level.Fatal;
            flts.TextColor = Color.White;
            flts.BackColor = Color.Red;
            flts.PointSize = 8.0f;
            rba.AddMapping(flts);

            log4net.Config.BasicConfigurator.Configure(rba);
            rba.ActivateOptions();
            #endregion RichTextBoxAppender

            #region RollingFileAppender
            log4net.Appender.RollingFileAppender fa = new log4net.Appender.RollingFileAppender();
            fa.AppendToFile = true;
            fa.Threshold = log4net.Core.Level.All;
            fa.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
            fa.MaxFileSize = 100000;
            fa.MaxSizeRollBackups = 10;
            fa.File = @".\LogPath\K-S2-Remote_";
            fa.DatePattern = @"yyyyMMdd.lo\g";
            fa.StaticLogFileName = false;

            // fa.Layout = new log4net.Layout.PatternLayout("%date{dd-MM-yyyy HH:mm:ss.fff} %5level %message (%logger{1}:%line)%n");
            log4net.Layout.PatternLayout pl = new log4net.Layout.PatternLayout(
                "%date [%thread] %-5level %message    (%logger)%newline");
            pl.Header = "######### K-S2-Remote_ gestartet ############\n";
            pl.Footer = "######### K-S2-Remote_ gestoppt  ############\n";
            fa.Layout = pl;

            log4net.Config.BasicConfigurator.Configure(fa);
            fa.ActivateOptions();
            #endregion RollingFileAppender
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            InitLog();
        }

        /// <summary>
        /// Closing MainForm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Stopping threads
            if (thumbnailtread != null)
            {
                thumbnailtread.Stop();
                thumbnailtread = null;
            }
            if (mjsource != null)
            {
                mjsource.Stop();
                mjsource = null;
            }

            Properties.Settings.Default.WindowSize = this.Size;
            Properties.Settings.Default.WindowLocation = this.Location;

            Properties.Settings.Default.splitContainerDebugPosition = splitContainerDebug.SplitterDistance;
            Properties.Settings.Default.splitContainerPictureviewPosition = splitContainerPictureview.SplitterDistance;

            Properties.Settings.Default.Save();
            

            log.Debug("Application closing");
        }

        /// <summary>
        /// Loading MainForm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            log.Debug("Application started");
            // Set window size
            if (Properties.Settings.Default.WindowSize != null)
            {
                //Check if window has a minimum size (Default 863; 523)
                if ((Properties.Settings.Default.WindowSize.Height > 480) &&
                    (Properties.Settings.Default.WindowSize.Width > 640))
                {
                    this.Size = Properties.Settings.Default.WindowSize;
                }
            }

            // Set window location
            if (Properties.Settings.Default.WindowLocation != null)
            {
                //Check if Window is visible
                if ((SystemInformation.VirtualScreen.Right > Properties.Settings.Default.WindowLocation.X + 50) &&
                    (SystemInformation.VirtualScreen.Bottom > Properties.Settings.Default.WindowLocation.Y + 50))
                {
                    this.Location = Properties.Settings.Default.WindowLocation;
                }
            }

            //Debug Window initial not visible
            splitContainerDebug.Panel2Collapsed = true;

            splitContainerDebug.SplitterDistance = Properties.Settings.Default.splitContainerDebugPosition;
            //You have to select the tab with the splitter first. Otherwise it will not work
            tabControl1.SelectedIndex = 1;
            splitContainerPictureview.SplitterDistance = Properties.Settings.Default.splitContainerPictureviewPosition;
            tabControl1.SelectedIndex = 0;


            if(Properties.Settings.Default.SimulateCamera)
                ks2 = new PtxK_S2.K_S2("");
            else 
                ks2 = new PtxK_S2.K_S2(Properties.Settings.Default.IPAdressCamera);

            tsslMessage.Text = "";

            btnGetParameter_Click(this, null); //Get parameter at program start

        }
        #endregion Form functions

        #region MenuItems
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tsmiOptions_Click(object sender, EventArgs e)
        {
            FormOptions fo = new FormOptions();
            fo.IPAddrCamera = Properties.Settings.Default.IPAdressCamera;
            fo.SimulateCamera = Properties.Settings.Default.SimulateCamera;
            DialogResult res = fo.ShowDialog();
            if (res == DialogResult.OK)
            {
                Properties.Settings.Default.IPAdressCamera = fo.IPAddrCamera;
                Properties.Settings.Default.SimulateCamera = fo.SimulateCamera;
                toolStripStatusLabel1.Text = Properties.Settings.Default.IPAdressCamera;
            }

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout fa = new FormAbout();
            fa.ShowDialog();
        }
        #endregion MenuItems

        #region Helper
        private void WaitCursor()
        {
            oldcur = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
        }
        
        private void RestoreCursor()
        {
            this.Cursor = oldcur;
        }

        #region Debug Helper
        private void splitContainerDebug_SplitterMoved(object sender, SplitterEventArgs e)
        {
            log.DebugFormat("Deb.Splitter:{0}", splitContainerDebug.SplitterDistance);
        }

        private void splitContainerPictureview_SplitterMoved(object sender, SplitterEventArgs e)
        {
            log.DebugFormat("Pic.Splitter:{0}", splitContainerPictureview.SplitterDistance);
        }
        #endregion Debug Helper
        #endregion Helper

        #region Kamera functions
        /// <summary>
        /// Take a Picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShoot_Click(object sender, EventArgs e)
        {
            string param;
            
            WaitCursor();
                        
            switch (getCheckedRadioButton(gbAutofocus))
            {
                case 2: param = "af=auto"; break;
                case 1: param = "af=on"; break;
                case 0: param = "af=off"; break;
                default: param = ""; break;
            }
            
            log.DebugFormat("Shoot param: {0}", param);

            if (!ks2.Shoot(param))
            {
                tbDebug.Text = ks2.ErrCode;
                tsslMessage.Text = "Takeing image failed";
            }
            else
            {
                tbDebug.Text = ks2.Content;

                //Get latest image infos
                //Check, if Image is ready
                int timeout = 30;
                bool success = true;
                while ((timeout > 0) && (success = ks2.GetImageInfo("latest", "")) && (!ks2.PhotoInfo.captured))
                {
                    timeout--;
                    Thread.Sleep(250);
                }

                //Get latest image and display at liveview picturebox
                if (success)
                {
                    tsslMessage.Text = "Takeing image success";
                    tbDebug.Text = ks2.Content;
                    if (cbLiveView.Checked == false)
                    {
                        pbLiveView.Image = ks2.GetImage(ks2.PhotoInfo.dir, ks2.PhotoInfo.file, "view");
                    }
                }
                else
                {
                    tbDebug.Text = ks2.ErrCode;
                    tsslMessage.Text = "Coudn't get Image";
                }
            }

            
            RestoreCursor();
        }

        private int getCheckedRadioButton(Control c)
        {
            int i;
            try
            {
                Control.ControlCollection cc = c.Controls;
                for (i = 0; i < cc.Count; i++)
                {
                    RadioButton rb = cc[i] as RadioButton;
                    if (rb.Checked)
                    {
                        return i;
                    }
                }
            }
            catch
            {
                i = -1;
            }
            return i;
        }


        /// <summary>
        /// Get the Camera Parameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetParameter_Click(object sender, EventArgs e)
        {
            log.Debug("Get Parameter");

            WaitCursor();

            if (ks2.GetParameter())
            {
                UpdateParameter(ks2.Parameter);
                tbDebug.Text = ks2.Content;
                tsslMessage.Text = "Get parameter success";
            }
            else
            {
                tbDebug.Text = ks2.ErrCode;
                tsslMessage.Text = "Get parameter failed";
            }

            RestoreCursor();
        }

        /// <summary>
        /// A Parameter has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (propsset) return; //We are setting parameter, right now. Do nothing!

            log.Debug("change camera params");
            
            WaitCursor();

            if(ks2.SetParameter(GetChangedParam(sender, e) ))
            {
                UpdateParameter(ks2.Parameter);
                tbDebug.Text = ks2.Content;
                tsslMessage.Text = "Change parameter success";
            }
            else
            {
                tbDebug.Text = ks2.ErrCode;
                tsslMessage.Text = "Change parameter failed";
            }
            
            RestoreCursor();
        }
        
        /// <summary>
        /// Set all values of all Controls from the parameter class
        /// </summary>
        /// <param name="parameter"></param>
        private void UpdateParameter(PtxK_S2.parameter parameter)
        {
            
            propsset = true;
            cbAv.DataSource = parameter.avList;
            cbAv.Text = parameter.av;
            cbTv.DataSource = parameter.tvList;
            cbTv.Text = parameter.tv;
            cbSv.DataSource = parameter.svList;
            cbSv.Text = parameter.sv;
            cbXv.DataSource = parameter.xvList;
            cbXv.Text = parameter.xv;
            if ((parameter.WBModeList != null)&&(parameter.WBModeList.Count > 0))
            {
                cbWB.DataSource = parameter.WBModeList;
                cbShootMode.DataSource = parameter.shootModeList;
                cbExposureMode.DataSource = parameter.exposureModeList;
                cbStillSize.DataSource = parameter.stillSizeList;
                cbMovieSize.DataSource = parameter.movieSizeList;
                cbEffect.DataSource = parameter.effectList;
                cbFilter.DataSource = parameter.filterList;
                
                lModel.Text = parameter.model;
                lFirmware.Text = parameter.firmwareVersion;
                lMACAddr.Text = parameter.macAddress;
                lSN.Text = parameter.serialNo;

                lSSID.Text = parameter.ssid;
                lKey.Text = parameter.key;
                lChannel.Text = parameter.channel; //TODO: Channellist
                lLiveState.Text = parameter.liveState;
                
            }
            cbWB.Text = parameter.WBMode;
            cbShootMode.Text = parameter.shootMode;
            cbExposureMode.Text = parameter.exposureMode;
            cbStillSize.Text = parameter.stillSize;
            cbMovieSize.Text = parameter.movieSize;
            cbEffect.Text = parameter.effect;
            cbFilter.Text = parameter.filter;

            propsset = false;
        }

        /// <summary>
        /// Determine wich parameter has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns>parameter string</returns>
        private string GetChangedParam(object sender, EventArgs e)
        {
            string s = "";
            if (sender == cbAv) s = "av=" + cbAv.Text;
            else if (sender == cbAv) s = "av=" + cbAv.Text;
            else if (sender == cbTv) s = "tv=" + cbTv.Text;
            else if (sender == cbSv) s = "sv=" + cbSv.Text;
            else if (sender == cbXv) s = "xv=" + cbXv.Text;

            else if (sender == cbWB) s = "WBMode=" + cbWB.Text;
            else if (sender == cbShootMode) s = "shootMode=" + cbShootMode.Text; //Don't work
            else if (sender == cbExposureMode) s = "exposureMode=" + cbExposureMode.Text; //Don't work
            else if (sender == cbStillSize) s = "stillSize=" + cbStillSize.Text;
            else if (sender == cbMovieSize) s = "movieSize=" + cbMovieSize.Text;
            else if (sender == cbEffect) s = "effect=" + cbEffect.Text;
            else if (sender == cbFilter) s = "filter=" + cbFilter.Text; //Don't work for dfl_extractColor, dfl_replaceColor
            return s;
        }

        /// <summary>
        /// Send the K-S2 green Button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGreen_Click(object sender, EventArgs e)
        {
            cb_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// Gets the Filelist from K-S2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetFileList_Click(object sender, EventArgs e)
        {
            log.Debug("Get Filelist");
            tsslMessage.Text = "Get filelist";

            WaitCursor();

            if (ks2.GetFilelist())
            {
                TreeModel tmodel = new TreeModel();
                tvaFiles.Model = tmodel;

                Bitmap folderimg = Properties.Resources.folder;
                Color backColor = folderimg.GetPixel(1, 1);
                folderimg.MakeTransparent(backColor);

                foreach (PtxK_S2.dirs dirs in ks2.Filelist.dirs)
                {
                    Node dirnode = new Node();
                    dirnode.Text = dirs.name;

                    dirnode.Image = folderimg;
                    
                    tmodel.Root.Nodes.Add(dirnode);

                    foreach (string fn in dirs.files)
                    {
                        Node file = new Node(fn);
                        file.Image = Properties.Resources.image_loading;
                        dirnode.Nodes.Add(file);

                    }
                }

                tsslMessage.Text = "Thumbnail download started.";

                if(Properties.Settings.Default.SimulateCamera)
                    thumbnailtread = new Thumbnails("",ks2.Filelist);
                else 
                    thumbnailtread = new Thumbnails(Properties.Settings.Default.IPAdressCamera,ks2.Filelist);

                thumbnailtread.NewFrame += thumbnailtread_NewFrame;
                thumbnailtread.Start();
                

                tbDebug.Text = ks2.Content;
                tsslMessage.Text = "Get file list success";
            }
            else
            {
                tbDebug.Text = ks2.ErrCode;
                tsslMessage.Text = "Get file list failed";
            }

            RestoreCursor();
        }

        #region Thumbnails
        void thumbnailtread_NewFrame(object sender, ThumbnailEventArgs e)
        {
            if (tvaFiles.InvokeRequired)
            {
                tvaFiles.Invoke(new MethodInvoker(
                delegate()
                {
                    SetThumbnail(e);
                }));
            }
            else
            {
                SetThumbnail(e);
            }
        }

        private void SetThumbnail(ThumbnailEventArgs tnea)
        {
            //Get Node from dir and filename
            Node n = GetNode(tnea.Filename);
            if(n != null)
            {
                tspbThumbnailsLoad.Minimum = 0;
                tspbThumbnailsLoad.Maximum = tnea.TotalCount;
                tspbThumbnailsLoad.Value = tnea.Count;

                n.Image = ks2.ResizeImage(tnea.Bitmap, 64, 48);

                if (tnea.TotalCount - 1 == tnea.Count)
                {
                    tspbThumbnailsLoad.Value = 0;
                    tsslMessage.Text = "Thumbnail download done.";
                }
            }
        }

        private Node GetNode(string filename)
        {
            Node n = null;
            TreeModel tm = (TreeModel)tvaFiles.Model;
            
            string[] filepath = filename.Split(new char[]{'/','\\'});
            if (filepath.Length < 2) return n;

            //log.DebugFormat("GetNode {0}/{1}",dir,filename);
            foreach (Node dn in tm.Nodes)
            {
                if (dn.Text == filepath[0])
                {
                    foreach (Node fn in dn.Nodes)
                    {
                        if (fn.Text == filepath[1])
                        {
                            n = fn;
                            //log.DebugFormat("Found Node {0}/{1}", dir, filename);
                            break;
                        }
                    }
                }

            }
            return n;
        }
        #endregion Thumbnails

        /// <summary>
        /// DoubleClick on Treeview Node. Opens the Image in Preview Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvaFiles_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Control.Parent != null)
            {
                if ((e.Control is NodeTextBox) || (e.Control is NodeIcon))
                {
                    if (e.Node.Parent.Tag != null)
                    {
                        
                        log.Debug(e.Node.Parent.Tag.ToString());
                        log.Debug(e.Node.Tag.ToString());
                                                
                        WaitCursor();
                        
                        if(e.Button == MouseButtons.Left)
                            pbPreview.Image = ks2.GetImage(e.Node.Parent.Tag.ToString(), e.Node.Tag.ToString(), "view");
                        else
                            pbPreview.Image = ks2.GetImage(e.Node.Parent.Tag.ToString(), e.Node.Tag.ToString(), "full");

                        tstbImagePath.Text = e.Node.Parent.Tag.ToString() + "/" + e.Node.Tag.ToString();
                        tstbImageSize.Text = pbPreview.Image.Size.Width.ToString() + "x" + pbPreview.Image.Size.Height.ToString();
                        RestoreCursor();
                    }
                }
            }
        }

        /// <summary>
        /// Click on Treeview Node for Checkbox toggle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvaFiles_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Control is NodeCheckBox)
            {
                log.Debug("Checkbox click: "+e.Node.Tag.ToString());
                if (e.Node.Parent.Tag == null) //DIR
                {
                    foreach (TreeNodeAdv tna in e.Node.Children)
                    {
                        ((Node)(tna.Tag)).CheckState = ((Node)(e.Node.Tag)).CheckState;
                    }
                }
                else
                {
                }
            }
        }

        private void tsbImageSaveAs_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Properties.Settings.Default.LastImageSaveDir;
            saveFileDialog1.FileName = Path.GetFileName(tstbImagePath.Text);

            DialogResult res = saveFileDialog1.ShowDialog();
            
            if (res == DialogResult.OK)
            {
                WaitCursor();
                string url = string.Format("http://{0}/v1/photos/" + tstbImagePath.Text, Properties.Settings.Default.IPAdressCamera);
                
                PtxK_S2.http cam = new PtxK_S2.http();
                if (cam.DownloadRemoteImageFile(url, saveFileDialog1.FileName))
                {
                    tsslMessage.Text = "Image download done";
                }
                else
                {
                    tsslMessage.Text = "Image download fail";
                }

                Properties.Settings.Default.LastImageSaveDir = Path.GetFullPath(saveFileDialog1.FileName);
                RestoreCursor();
            }
        }
        #endregion Kamera functions

        #region LiveView
        private void cbLiveView_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                mjsource = new PtxK_S2.MJPEGSource();
                mjsource.VideoSource = string.Format("http://{0}/v1/liveview",Properties.Settings.Default.IPAdressCamera); 
                mjsource.NewFrame += mjsource_NewFrame;
                mjsource.Start();

            }
            else
            {
                mjsource.Stop();
                mjsource = null;
            }
        }

        void mjsource_NewFrame(object sender, PtxK_S2.CameraEventArgs e)
        {
            if (pbLiveView.InvokeRequired)
            {
                pbLiveView.Invoke(new MethodInvoker(
                delegate()
                {
                    pbLiveView.Image = ks2.ResizeImage(e.Bitmap,720,480); 
                }));
            }
            else
            {
                pbLiveView.Image = ks2.ResizeImage(e.Bitmap, 720, 480);
            }
            //pbLiveView.Image = e.Bitmap;
        }
        #endregion LiveView

        private void tvaFiles_Collapsing(object sender, TreeViewAdvEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                if (e.Node.Level == 1) //DIR!
                {
                    log.DebugFormat("Collapsing {0}", ((Node)(e.Node.Tag)).Text);
                    ((Node)(e.Node.Tag)).Image = Properties.Resources.folder;
                }

            }
        }

        private void tvaFiles_Expanding(object sender, TreeViewAdvEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                if (e.Node.Level == 1) //DIR!
                {
                    log.DebugFormat("Expanding {0}", ((Node)(e.Node.Tag)).Text);
                    ((Node)(e.Node.Tag)).Image = Properties.Resources.folder_open;
                }
            }
        }

        private void tsbDebug_Click(object sender, EventArgs e)
        {
            if (splitContainerDebug.Panel2Collapsed)
            {
                splitContainerDebug.Panel2Collapsed = false;
                tsbDebug.Image = Properties.Resources.lightningbug_on;
                tsslMessage.Text = "Debug window visible";
            }
            else
            {
                splitContainerDebug.Panel2Collapsed = true;
                tsbDebug.Image = Properties.Resources.lightningbug_off;
                tsslMessage.Text = "Debug windows hidden";

            }
        }

        private void clbAF_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = (sender as CheckedListBox).SelectedIndex;
            log.DebugFormat("AF={0}", idx);
        }
    }
}
