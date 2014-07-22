namespace SimpleBuilderNamespace
{
    partial class SimpleBuilder
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
            this.btnBuildBox = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbox_Size = new System.Windows.Forms.TextBox();
            this.tbox_Distance = new System.Windows.Forms.TextBox();
            this.lstPrims = new Radegast.ListViewNoFlicker();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.rotZ = new System.Windows.Forms.NumericUpDown();
            this.rotY = new System.Windows.Forms.NumericUpDown();
            this.rotX = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.scaleZ = new System.Windows.Forms.NumericUpDown();
            this.scaleY = new System.Windows.Forms.NumericUpDown();
            this.scaleX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numRadius = new System.Windows.Forms.NumericUpDown();
            this.button7 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rotZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBuildBox
            // 
            this.btnBuildBox.Location = new System.Drawing.Point(18, 17);
            this.btnBuildBox.Name = "btnBuildBox";
            this.btnBuildBox.Size = new System.Drawing.Size(159, 23);
            this.btnBuildBox.TabIndex = 0;
            this.btnBuildBox.Tag = "";
            this.btnBuildBox.Text = "Box";
            this.btnBuildBox.UseVisualStyleBackColor = true;
            this.btnBuildBox.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(213, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(159, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Sphere";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(213, 46);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(159, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Torus";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(213, 75);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(159, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Tube";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(18, 75);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(159, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "Prism";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(18, 46);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(159, 23);
            this.button5.TabIndex = 5;
            this.button5.Text = "Cylinder";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(210, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Distance";
            // 
            // tbox_Size
            // 
            this.tbox_Size.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbox_Size.Location = new System.Drawing.Point(313, 113);
            this.tbox_Size.MaxLength = 5;
            this.tbox_Size.Name = "tbox_Size";
            this.tbox_Size.Size = new System.Drawing.Size(59, 20);
            this.tbox_Size.TabIndex = 10;
            this.tbox_Size.Text = "0.500";
            // 
            // tbox_Distance
            // 
            this.tbox_Distance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbox_Distance.Location = new System.Drawing.Point(118, 113);
            this.tbox_Distance.MaxLength = 5;
            this.tbox_Distance.Name = "tbox_Distance";
            this.tbox_Distance.Size = new System.Drawing.Size(59, 20);
            this.tbox_Distance.TabIndex = 11;
            this.tbox_Distance.Text = "1.000";
            // 
            // lstPrims
            // 
            this.lstPrims.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstPrims.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstPrims.FullRowSelect = true;
            this.lstPrims.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstPrims.HideSelection = false;
            this.lstPrims.LabelWrap = false;
            this.lstPrims.Location = new System.Drawing.Point(392, 43);
            this.lstPrims.MultiSelect = false;
            this.lstPrims.Name = "lstPrims";
            this.lstPrims.ShowGroups = false;
            this.lstPrims.Size = new System.Drawing.Size(274, 262);
            this.lstPrims.TabIndex = 12;
            this.lstPrims.UseCompatibleStateImageBehavior = false;
            this.lstPrims.View = System.Windows.Forms.View.Details;
            this.lstPrims.VirtualMode = true;
            this.lstPrims.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lstPrims_RetrieveVirtualItem);
            this.lstPrims.SelectedIndexChanged += new System.EventHandler(this.lstPrims_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 270;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.rotZ);
            this.groupBox1.Controls.Add(this.rotY);
            this.groupBox1.Controls.Add(this.rotX);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.scaleZ);
            this.groupBox1.Controls.Add(this.scaleY);
            this.groupBox1.Controls.Add(this.scaleX);
            this.groupBox1.Location = new System.Drawing.Point(18, 152);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(354, 155);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transform";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(272, 129);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 24;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Rotate ( X / Y / Z )";
            // 
            // rotZ
            // 
            this.rotZ.DecimalPlaces = 4;
            this.rotZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rotZ.Location = new System.Drawing.Point(288, 51);
            this.rotZ.Name = "rotZ";
            this.rotZ.Size = new System.Drawing.Size(60, 20);
            this.rotZ.TabIndex = 22;
            // 
            // rotY
            // 
            this.rotY.DecimalPlaces = 4;
            this.rotY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rotY.Location = new System.Drawing.Point(209, 51);
            this.rotY.Name = "rotY";
            this.rotY.Size = new System.Drawing.Size(60, 20);
            this.rotY.TabIndex = 21;
            // 
            // rotX
            // 
            this.rotX.DecimalPlaces = 4;
            this.rotX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rotX.Location = new System.Drawing.Point(126, 53);
            this.rotX.Name = "rotX";
            this.rotX.Size = new System.Drawing.Size(60, 20);
            this.rotX.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Scale ( X / Y / Z )";
            // 
            // scaleZ
            // 
            this.scaleZ.DecimalPlaces = 4;
            this.scaleZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.scaleZ.Location = new System.Drawing.Point(288, 19);
            this.scaleZ.Name = "scaleZ";
            this.scaleZ.Size = new System.Drawing.Size(60, 20);
            this.scaleZ.TabIndex = 18;
            // 
            // scaleY
            // 
            this.scaleY.DecimalPlaces = 4;
            this.scaleY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.scaleY.Location = new System.Drawing.Point(209, 19);
            this.scaleY.Name = "scaleY";
            this.scaleY.Size = new System.Drawing.Size(60, 20);
            this.scaleY.TabIndex = 17;
            // 
            // scaleX
            // 
            this.scaleX.DecimalPlaces = 4;
            this.scaleX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.scaleX.Location = new System.Drawing.Point(126, 21);
            this.scaleX.Name = "scaleX";
            this.scaleX.Size = new System.Drawing.Size(60, 20);
            this.scaleX.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(392, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Search radius in meter:";
            // 
            // numRadius
            // 
            this.numRadius.Location = new System.Drawing.Point(513, 15);
            this.numRadius.Name = "numRadius";
            this.numRadius.Size = new System.Drawing.Size(60, 20);
            this.numRadius.TabIndex = 15;
            this.numRadius.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(591, 15);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 25;
            this.button7.Text = "Refresh";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // SimpleBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button7);
            this.Controls.Add(this.numRadius);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lstPrims);
            this.Controls.Add(this.tbox_Distance);
            this.Controls.Add(this.tbox_Size);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnBuildBox);
            this.Name = "SimpleBuilder";
            this.Size = new System.Drawing.Size(697, 349);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rotZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRadius)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBuildBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbox_Size;
        private System.Windows.Forms.TextBox tbox_Distance;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numRadius;
        public System.Windows.Forms.ColumnHeader columnHeader1;
        public Radegast.ListViewNoFlicker lstPrims;
        private System.Windows.Forms.NumericUpDown scaleZ;
        private System.Windows.Forms.NumericUpDown scaleY;
        private System.Windows.Forms.NumericUpDown scaleX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown rotZ;
        private System.Windows.Forms.NumericUpDown rotY;
        private System.Windows.Forms.NumericUpDown rotX;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button button7;
    }
}
