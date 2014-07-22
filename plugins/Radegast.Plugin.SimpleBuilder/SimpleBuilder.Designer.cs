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
            this.lstPrims = new Radegast.ListViewNoFlicker();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupTransform = new System.Windows.Forms.GroupBox();
            this.posZ = new System.Windows.Forms.NumericUpDown();
            this.posY = new System.Windows.Forms.NumericUpDown();
            this.posX = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
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
            this.tbox_Distance = new System.Windows.Forms.NumericUpDown();
            this.tbox_Size = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_ObjectName = new System.Windows.Forms.TextBox();
            this.groupTransform.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.posZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbox_Distance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbox_Size)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBuildBox
            // 
            this.btnBuildBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuildBox.Location = new System.Drawing.Point(17, 73);
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
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(212, 73);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(159, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Sphere";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(212, 102);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(159, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Torus";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(212, 131);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(159, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Tube";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(17, 131);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(159, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "Prism";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // button5
            // 
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Location = new System.Drawing.Point(17, 102);
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
            this.label1.Location = new System.Drawing.Point(209, 169);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Initial Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 171);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Initial Distance";
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
            this.lstPrims.Location = new System.Drawing.Point(391, 71);
            this.lstPrims.MultiSelect = false;
            this.lstPrims.Name = "lstPrims";
            this.lstPrims.ShowGroups = false;
            this.lstPrims.Size = new System.Drawing.Size(274, 292);
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
            // groupTransform
            // 
            this.groupTransform.Controls.Add(this.posZ);
            this.groupTransform.Controls.Add(this.posY);
            this.groupTransform.Controls.Add(this.posX);
            this.groupTransform.Controls.Add(this.label9);
            this.groupTransform.Controls.Add(this.label8);
            this.groupTransform.Controls.Add(this.label7);
            this.groupTransform.Controls.Add(this.label6);
            this.groupTransform.Controls.Add(this.btnSave);
            this.groupTransform.Controls.Add(this.label5);
            this.groupTransform.Controls.Add(this.rotZ);
            this.groupTransform.Controls.Add(this.rotY);
            this.groupTransform.Controls.Add(this.rotX);
            this.groupTransform.Controls.Add(this.label4);
            this.groupTransform.Controls.Add(this.scaleZ);
            this.groupTransform.Controls.Add(this.scaleY);
            this.groupTransform.Controls.Add(this.scaleX);
            this.groupTransform.Location = new System.Drawing.Point(17, 208);
            this.groupTransform.Name = "groupTransform";
            this.groupTransform.Size = new System.Drawing.Size(354, 155);
            this.groupTransform.TabIndex = 13;
            this.groupTransform.TabStop = false;
            this.groupTransform.Text = "Transform";
            // 
            // posZ
            // 
            this.posZ.DecimalPlaces = 5;
            this.posZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.posZ.Location = new System.Drawing.Point(261, 90);
            this.posZ.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.posZ.Name = "posZ";
            this.posZ.Size = new System.Drawing.Size(87, 20);
            this.posZ.TabIndex = 31;
            this.posZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // posY
            // 
            this.posY.DecimalPlaces = 5;
            this.posY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.posY.Location = new System.Drawing.Point(168, 90);
            this.posY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.posY.Name = "posY";
            this.posY.Size = new System.Drawing.Size(87, 20);
            this.posY.TabIndex = 30;
            this.posY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // posX
            // 
            this.posX.DecimalPlaces = 5;
            this.posX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.posX.Location = new System.Drawing.Point(76, 90);
            this.posX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.posX.Name = "posX";
            this.posX.Size = new System.Drawing.Size(86, 20);
            this.posX.TabIndex = 29;
            this.posX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(15, 92);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "Position";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label8.Location = new System.Drawing.Point(292, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(15, 15);
            this.label8.TabIndex = 27;
            this.label8.Text = "Z";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.label7.Location = new System.Drawing.Point(203, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 15);
            this.label7.TabIndex = 26;
            this.label7.Text = "Y";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label6.Location = new System.Drawing.Point(106, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 15);
            this.label6.TabIndex = 25;
            this.label6.Text = "X";
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(273, 126);
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
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Rotate";
            // 
            // rotZ
            // 
            this.rotZ.DecimalPlaces = 5;
            this.rotZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rotZ.Location = new System.Drawing.Point(261, 64);
            this.rotZ.Name = "rotZ";
            this.rotZ.Size = new System.Drawing.Size(87, 20);
            this.rotZ.TabIndex = 22;
            this.rotZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // rotY
            // 
            this.rotY.DecimalPlaces = 5;
            this.rotY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rotY.Location = new System.Drawing.Point(168, 64);
            this.rotY.Name = "rotY";
            this.rotY.Size = new System.Drawing.Size(87, 20);
            this.rotY.TabIndex = 21;
            this.rotY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // rotX
            // 
            this.rotX.DecimalPlaces = 5;
            this.rotX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.rotX.Location = new System.Drawing.Point(76, 64);
            this.rotX.Name = "rotX";
            this.rotX.Size = new System.Drawing.Size(86, 20);
            this.rotX.TabIndex = 20;
            this.rotX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Scale";
            // 
            // scaleZ
            // 
            this.scaleZ.DecimalPlaces = 5;
            this.scaleZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.scaleZ.Location = new System.Drawing.Point(261, 39);
            this.scaleZ.Name = "scaleZ";
            this.scaleZ.Size = new System.Drawing.Size(87, 20);
            this.scaleZ.TabIndex = 18;
            this.scaleZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // scaleY
            // 
            this.scaleY.DecimalPlaces = 5;
            this.scaleY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.scaleY.Location = new System.Drawing.Point(168, 39);
            this.scaleY.Name = "scaleY";
            this.scaleY.Size = new System.Drawing.Size(87, 20);
            this.scaleY.TabIndex = 17;
            this.scaleY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // scaleX
            // 
            this.scaleX.DecimalPlaces = 5;
            this.scaleX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.scaleX.Location = new System.Drawing.Point(76, 39);
            this.scaleX.Name = "scaleX";
            this.scaleX.Size = new System.Drawing.Size(86, 20);
            this.scaleX.TabIndex = 16;
            this.scaleX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(389, 20);
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
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Location = new System.Drawing.Point(591, 15);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 25;
            this.button7.Text = "Refresh";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // tbox_Distance
            // 
            this.tbox_Distance.DecimalPlaces = 5;
            this.tbox_Distance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.tbox_Distance.Location = new System.Drawing.Point(90, 169);
            this.tbox_Distance.Name = "tbox_Distance";
            this.tbox_Distance.Size = new System.Drawing.Size(86, 20);
            this.tbox_Distance.TabIndex = 26;
            this.tbox_Distance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbox_Distance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tbox_Size
            // 
            this.tbox_Size.DecimalPlaces = 5;
            this.tbox_Size.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.tbox_Size.Location = new System.Drawing.Point(285, 164);
            this.tbox_Size.Name = "tbox_Size";
            this.tbox_Size.Size = new System.Drawing.Size(86, 20);
            this.tbox_Size.TabIndex = 27;
            this.tbox_Size.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbox_Size.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(252, 13);
            this.label10.TabIndex = 28;
            this.label10.Text = "Build a prim by pressing one of these Buttons below:";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(17, 366);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(82, 13);
            this.lblVersion.TabIndex = 30;
            this.lblVersion.Text = "Version 0.0.3";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(16, 46);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 13);
            this.label11.TabIndex = 31;
            this.label11.Text = "Object Name";
            // 
            // txt_ObjectName
            // 
            this.txt_ObjectName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_ObjectName.Location = new System.Drawing.Point(102, 43);
            this.txt_ObjectName.MaxLength = 63;
            this.txt_ObjectName.Name = "txt_ObjectName";
            this.txt_ObjectName.Size = new System.Drawing.Size(269, 20);
            this.txt_ObjectName.TabIndex = 32;
            // 
            // SimpleBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txt_ObjectName);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbox_Size);
            this.Controls.Add(this.tbox_Distance);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.numRadius);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupTransform);
            this.Controls.Add(this.lstPrims);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnBuildBox);
            this.Name = "SimpleBuilder";
            this.Size = new System.Drawing.Size(679, 392);
            this.groupTransform.ResumeLayout(false);
            this.groupTransform.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.posZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbox_Distance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbox_Size)).EndInit();
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
        private System.Windows.Forms.GroupBox groupTransform;
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
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown posZ;
        private System.Windows.Forms.NumericUpDown posY;
        private System.Windows.Forms.NumericUpDown posX;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown tbox_Distance;
        private System.Windows.Forms.NumericUpDown tbox_Size;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_ObjectName;
    }
}
