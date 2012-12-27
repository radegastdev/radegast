namespace Radegast.Rendering
{
    partial class GraphicsPreferences
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
            this.cbAA = new System.Windows.Forms.CheckBox();
            this.chkWireFrame = new System.Windows.Forms.CheckBox();
            this.lblDrawDistance = new System.Windows.Forms.Label();
            this.tbDrawDistance = new System.Windows.Forms.TrackBar();
            this.cbWaterReflections = new System.Windows.Forms.CheckBox();
            this.cbOcclusionCulling = new System.Windows.Forms.CheckBox();
            this.cbShiny = new System.Windows.Forms.CheckBox();
            this.cbVBO = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbDrawDistance)).BeginInit();
            this.SuspendLayout();
            // 
            // cbAA
            // 
            this.cbAA.AutoSize = true;
            this.cbAA.Location = new System.Drawing.Point(3, 26);
            this.cbAA.Name = "cbAA";
            this.cbAA.Size = new System.Drawing.Size(160, 17);
            this.cbAA.TabIndex = 2;
            this.cbAA.Text = "Anti-aliasing (requires restart)";
            this.cbAA.UseVisualStyleBackColor = true;
            this.cbAA.CheckedChanged += new System.EventHandler(this.cbAA_CheckedChanged);
            // 
            // chkWireFrame
            // 
            this.chkWireFrame.AutoSize = true;
            this.chkWireFrame.Location = new System.Drawing.Point(3, 3);
            this.chkWireFrame.Name = "chkWireFrame";
            this.chkWireFrame.Size = new System.Drawing.Size(74, 17);
            this.chkWireFrame.TabIndex = 1;
            this.chkWireFrame.Text = "Wireframe";
            this.chkWireFrame.UseVisualStyleBackColor = true;
            this.chkWireFrame.CheckedChanged += new System.EventHandler(this.chkWireFrame_CheckedChanged);
            // 
            // lblDrawDistance
            // 
            this.lblDrawDistance.AutoSize = true;
            this.lblDrawDistance.Location = new System.Drawing.Point(187, 138);
            this.lblDrawDistance.Name = "lblDrawDistance";
            this.lblDrawDistance.Size = new System.Drawing.Size(93, 13);
            this.lblDrawDistance.TabIndex = 21;
            this.lblDrawDistance.Text = "Draw distance: 48";
            this.lblDrawDistance.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbDrawDistance
            // 
            this.tbDrawDistance.Location = new System.Drawing.Point(3, 115);
            this.tbDrawDistance.Maximum = 256;
            this.tbDrawDistance.Minimum = 32;
            this.tbDrawDistance.Name = "tbDrawDistance";
            this.tbDrawDistance.Size = new System.Drawing.Size(277, 45);
            this.tbDrawDistance.TabIndex = 20;
            this.tbDrawDistance.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbDrawDistance.Value = 48;
            this.tbDrawDistance.Scroll += new System.EventHandler(this.tbDrawDistance_Scroll);
            // 
            // cbWaterReflections
            // 
            this.cbWaterReflections.AutoSize = true;
            this.cbWaterReflections.Location = new System.Drawing.Point(3, 49);
            this.cbWaterReflections.Name = "cbWaterReflections";
            this.cbWaterReflections.Size = new System.Drawing.Size(111, 17);
            this.cbWaterReflections.TabIndex = 3;
            this.cbWaterReflections.Text = "Water Reflections";
            this.cbWaterReflections.UseVisualStyleBackColor = true;
            this.cbWaterReflections.CheckedChanged += new System.EventHandler(this.cbWaterReflections_CheckedChanged);
            // 
            // cbOcclusionCulling
            // 
            this.cbOcclusionCulling.AutoSize = true;
            this.cbOcclusionCulling.Location = new System.Drawing.Point(169, 3);
            this.cbOcclusionCulling.Name = "cbOcclusionCulling";
            this.cbOcclusionCulling.Size = new System.Drawing.Size(107, 17);
            this.cbOcclusionCulling.TabIndex = 5;
            this.cbOcclusionCulling.Text = "Occlusion Culling";
            this.cbOcclusionCulling.UseVisualStyleBackColor = true;
            this.cbOcclusionCulling.CheckedChanged += new System.EventHandler(this.cbOcclusionCulling_CheckedChanged);
            // 
            // cbShiny
            // 
            this.cbShiny.AutoSize = true;
            this.cbShiny.Location = new System.Drawing.Point(3, 72);
            this.cbShiny.Name = "cbShiny";
            this.cbShiny.Size = new System.Drawing.Size(52, 17);
            this.cbShiny.TabIndex = 4;
            this.cbShiny.Text = "Shiny";
            this.cbShiny.UseVisualStyleBackColor = true;
            this.cbShiny.CheckedChanged += new System.EventHandler(this.cbShiny_CheckedChanged);
            // 
            // cbVBO
            // 
            this.cbVBO.AutoSize = true;
            this.cbVBO.Location = new System.Drawing.Point(169, 26);
            this.cbVBO.Name = "cbVBO";
            this.cbVBO.Size = new System.Drawing.Size(70, 17);
            this.cbVBO.TabIndex = 6;
            this.cbVBO.Text = "Use VBO";
            this.cbVBO.UseVisualStyleBackColor = true;
            this.cbVBO.CheckedChanged += new System.EventHandler(this.cbVBO_CheckedChanged);
            // 
            // GraphicsPreferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbVBO);
            this.Controls.Add(this.cbShiny);
            this.Controls.Add(this.cbOcclusionCulling);
            this.Controls.Add(this.cbWaterReflections);
            this.Controls.Add(this.lblDrawDistance);
            this.Controls.Add(this.tbDrawDistance);
            this.Controls.Add(this.cbAA);
            this.Controls.Add(this.chkWireFrame);
            this.Name = "GraphicsPreferences";
            this.Size = new System.Drawing.Size(283, 160);
            ((System.ComponentModel.ISupportInitialize)(this.tbDrawDistance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox cbAA;
        public System.Windows.Forms.CheckBox chkWireFrame;
        public System.Windows.Forms.Label lblDrawDistance;
        public System.Windows.Forms.TrackBar tbDrawDistance;
        public System.Windows.Forms.CheckBox cbWaterReflections;
        private System.Windows.Forms.CheckBox cbOcclusionCulling;
        private System.Windows.Forms.CheckBox cbShiny;
        private System.Windows.Forms.CheckBox cbVBO;
    }
}
