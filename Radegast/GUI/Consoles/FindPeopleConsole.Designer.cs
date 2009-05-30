namespace Radegast
{
    partial class FindPeopleConsole
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lvwFindPeople = new System.Windows.Forms.ListView();
            this.chdName = new System.Windows.Forms.ColumnHeader();
            this.chdOnline = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // lvwFindPeople
            // 
            this.lvwFindPeople.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chdName,
            this.chdOnline});
            this.lvwFindPeople.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwFindPeople.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwFindPeople.Location = new System.Drawing.Point(0, 0);
            this.lvwFindPeople.MultiSelect = false;
            this.lvwFindPeople.Name = "lvwFindPeople";
            this.lvwFindPeople.Size = new System.Drawing.Size(474, 319);
            this.lvwFindPeople.TabIndex = 0;
            this.lvwFindPeople.UseCompatibleStateImageBehavior = false;
            this.lvwFindPeople.View = System.Windows.Forms.View.Details;
            this.lvwFindPeople.SelectedIndexChanged += new System.EventHandler(this.lvwFindPeople_SelectedIndexChanged);
            // 
            // chdName
            // 
            this.chdName.Text = "Name";
            this.chdName.Width = 205;
            // 
            // chdOnline
            // 
            this.chdOnline.Text = "Online";
            // 
            // FindPeopleConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvwFindPeople);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FindPeopleConsole";
            this.Size = new System.Drawing.Size(474, 319);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvwFindPeople;
        private System.Windows.Forms.ColumnHeader chdName;
        private System.Windows.Forms.ColumnHeader chdOnline;
    }
}
