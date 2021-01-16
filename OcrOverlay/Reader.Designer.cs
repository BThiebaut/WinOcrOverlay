
using System;
using System.Windows.Forms;

namespace OcrOverlay
{
    public partial class Reader : Form
    {

        private TextBox readerText;

        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.readerText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // readerText
            // 
            this.readerText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.readerText.Location = new System.Drawing.Point(0, 0);
            this.readerText.Multiline = true;
            this.readerText.Name = "readerText";
            this.readerText.ReadOnly = true;
            this.readerText.Size = this.Size;
            this.readerText.TabIndex = 0;
            // 
            // Reader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.readerText);
            this.Name = "Reader";
            this.ShowIcon = false;
            this.Text = "Reader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Reader_Closing);
            this.Shown += new System.EventHandler(this.SelectArea_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        
    }
}

