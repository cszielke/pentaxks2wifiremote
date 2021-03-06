﻿/*
 * This was based off the work I found of Stephanie Giovannini – GMAN Mar 20 '10 at 21:29
 * and 
 * http://stackoverflow.com/questions/2484822/richtextboxappender-using-log4net
*/

using System;
using System.Windows.Forms;
using System.Drawing;
using log4net;
using log4net.Core;
using log4net.Appender;
using log4net.Util;


namespace Pentax_K_S2_Remote
{
    class RichTextBoxAppender : AppenderSkeleton
    {
        #region Private Instance Fields
        private RichTextBox richTextBox = null;
        private LevelMapping levelMapping = new LevelMapping();
        private int maxTextLength = 100000;
        #endregion

        private delegate void UpdateControlDelegate(LoggingEvent loggingEvent);

        #region Constructor
        public RichTextBoxAppender(RichTextBox myRichTextBox) : base()
        {
            richTextBox = myRichTextBox;
        }
        #endregion

        private void UpdateControl(LoggingEvent loggingEvent)
        {
            if (!richTextBox.Visible) return;
            
            // There may be performance issues if the buffer gets too long
            // So periodically clear the buffer
            if (richTextBox.TextLength > maxTextLength)
            {
                //richTextBox.Clear();
                //richTextBox.SuspendLayout();
                while (richTextBox.TextLength > (int)(maxTextLength*0.9))
                {
                    richTextBox.Select(0, richTextBox.GetFirstCharIndexFromLine(1)); // select the first line
                    richTextBox.SelectedText = "";
                }

                //richTextBox.AppendText(string.Format("(Cleared log length max: {0})\n", maxTextLength));
                //richTextBox.ScrollToCaret();
                //richTextBox.ResumeLayout();
            }

            // look for a style mapping
            LevelTextStyle selectedStyle = levelMapping.Lookup(loggingEvent.Level) as LevelTextStyle;
            if (selectedStyle != null)
            {
                // set the colors of the text about to be appended
                richTextBox.SelectionBackColor = selectedStyle.BackColor;
                richTextBox.SelectionColor = selectedStyle.TextColor;

                // alter selection font as much as necessary
                // missing settings are replaced by the font settings on the control
                if (selectedStyle.Font != null)
                {
                    // set Font Family, size and styles
                    richTextBox.SelectionFont = selectedStyle.Font;
                }
                else if (selectedStyle.PointSize > 0 && richTextBox.Font.SizeInPoints != selectedStyle.PointSize)
                {
                    // use control's font family, set size and styles
                    float size = selectedStyle.PointSize > 0.0f ? selectedStyle.PointSize : richTextBox.Font.SizeInPoints;
                    richTextBox.SelectionFont = new Font(richTextBox.Font.FontFamily.Name, size, selectedStyle.FontStyle);
                }
                else if (richTextBox.Font.Style != selectedStyle.FontStyle)
                {
                    // use control's font family and size, set styles
                    richTextBox.SelectionFont = new Font(richTextBox.Font, selectedStyle.FontStyle);
                }
            }
            richTextBox.AppendText(RenderLoggingEvent(loggingEvent));
            richTextBox.ScrollToCaret();
        }

        protected override void Append(LoggingEvent LoggingEvent)
        {
            if (richTextBox.InvokeRequired)
            {
                richTextBox.Invoke(
                    new UpdateControlDelegate(UpdateControl),
                    new object[] { LoggingEvent });
            }
            else
            {
                UpdateControl(LoggingEvent);
            }
        }

        public void AddMapping(LevelTextStyle mapping)
        {
            levelMapping.Add(mapping);
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            levelMapping.ActivateOptions();
        }

        protected override bool RequiresLayout { get { return true; } }
    }

    public class LevelTextStyle : LevelMappingEntry
    {
        private Color textColor;
        private Color backColor;
        private FontStyle fontStyle = FontStyle.Regular;
        private float pointSize = 0.0f;
        private bool bold = false;
        private bool italic = false;
        private string fontFamilyName = null;
        private Font font = null;

        public bool Bold { get { return bold; } set { bold = value; } }
        public bool Italic { get { return italic; } set { italic = value; } }
        public float PointSize { get { return pointSize; } set { pointSize = value; } }

        /// <summary>
        /// Initialize the options for the object
        /// </summary>
        /// <remarks>Parse the properties</remarks>
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            if (bold) fontStyle |= FontStyle.Bold;
            if (italic) fontStyle |= FontStyle.Italic;

            if (fontFamilyName != null)
            {
                float size = pointSize > 0.0f ? pointSize : 8.25f;
                try
                {
                    font = new Font(fontFamilyName, size, fontStyle);
                }
                catch (Exception)
                {
                    font = new Font("Arial", 8.25f, FontStyle.Regular);
                }
            }
        }

        public Color TextColor { get { return textColor; } set { textColor = value; } }
        public Color BackColor { get { return backColor; } set { backColor = value; } }
        public FontStyle FontStyle { get { return fontStyle; } set { fontStyle = value; } }
        public Font Font { get { return font; } set { font = value; } }
    }
}
