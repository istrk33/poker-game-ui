namespace PokerProject
{
    partial class ExampleBetView
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.betAmount = new System.Windows.Forms.NumericUpDown();
            this.betAmounButton = new System.Windows.Forms.Button();
            this.betAllButton = new System.Windows.Forms.Button();
            this.checkButton = new System.Windows.Forms.Button();
            this.foldButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.betAmount)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // betAmount
            // 
            this.betAmount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.betAmount.Location = new System.Drawing.Point(2, 17);
            this.betAmount.Margin = new System.Windows.Forms.Padding(2, 17, 2, 2);
            this.betAmount.Name = "betAmount";
            this.betAmount.Size = new System.Drawing.Size(219, 31);
            this.betAmount.TabIndex = 0;
            // 
            // betAmounButton
            // 
            this.betAmounButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.betAmounButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.betAmounButton.Location = new System.Drawing.Point(225, 2);
            this.betAmounButton.Margin = new System.Windows.Forms.Padding(2);
            this.betAmounButton.Name = "betAmounButton";
            this.betAmounButton.Size = new System.Drawing.Size(220, 62);
            this.betAmounButton.TabIndex = 1;
            this.betAmounButton.Text = "Bet Amount";
            this.betAmounButton.UseVisualStyleBackColor = true;
            // 
            // betAllButton
            // 
            this.betAllButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.betAllButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.betAllButton.Location = new System.Drawing.Point(2, 146);
            this.betAllButton.Margin = new System.Windows.Forms.Padding(2);
            this.betAllButton.Name = "betAllButton";
            this.betAllButton.Size = new System.Drawing.Size(449, 68);
            this.betAllButton.TabIndex = 2;
            this.betAllButton.Text = "Bet All";
            this.betAllButton.UseVisualStyleBackColor = true;
            // 
            // checkButton
            // 
            this.checkButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.checkButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkButton.Location = new System.Drawing.Point(2, 2);
            this.checkButton.Margin = new System.Windows.Forms.Padding(2);
            this.checkButton.Name = "checkButton";
            this.checkButton.Size = new System.Drawing.Size(449, 68);
            this.checkButton.TabIndex = 3;
            this.checkButton.Text = "Check";
            this.checkButton.UseVisualStyleBackColor = true;
            // 
            // foldButton
            // 
            this.foldButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.foldButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.foldButton.Location = new System.Drawing.Point(2, 218);
            this.foldButton.Margin = new System.Windows.Forms.Padding(2);
            this.foldButton.Name = "foldButton";
            this.foldButton.Size = new System.Drawing.Size(449, 68);
            this.foldButton.TabIndex = 4;
            this.foldButton.Text = "Fold";
            this.foldButton.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.foldButton, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.checkButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.betAllButton, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(453, 288);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.betAmount, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.betAmounButton, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 75);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(447, 66);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // ExampleBetView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 288);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ExampleBetView";
            this.Text = "ExampleBetView";
            ((System.ComponentModel.ISupportInitialize)(this.betAmount)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown betAmount;
        private System.Windows.Forms.Button betAmounButton;
        private System.Windows.Forms.Button betAllButton;
        private System.Windows.Forms.Button checkButton;
        private System.Windows.Forms.Button foldButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}