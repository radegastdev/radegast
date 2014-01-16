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
            // SimpleBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            this.Size = new System.Drawing.Size(454, 286);
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
    }
}
