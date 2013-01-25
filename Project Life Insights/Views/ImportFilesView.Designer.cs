namespace ProjectLifeInsights.Views
{
    partial class ImportFilesView
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
            Telerik.WinControls.UI.GridViewBrowseColumn gridViewBrowseColumn1 = new Telerik.WinControls.UI.GridViewBrowseColumn();
            Telerik.WinControls.UI.GridViewImageColumn gridViewImageColumn1 = new Telerik.WinControls.UI.GridViewImageColumn();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn1 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewCommandColumn gridViewCommandColumn1 = new Telerik.WinControls.UI.GridViewCommandColumn();
            Telerik.WinControls.Data.SortDescriptor sortDescriptor1 = new Telerik.WinControls.Data.SortDescriptor();
            this.TelerikMetroBlueTheme = new Telerik.WinControls.Themes.TelerikMetroBlueTheme();
            this.FileList = new Telerik.WinControls.UI.RadGridView();
            this.ButtonImport = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.FileList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonImport)).BeginInit();
            this.SuspendLayout();
            // 
            // FileList
            // 
            this.FileList.AllowDrop = true;
            this.FileList.BackColor = System.Drawing.SystemColors.Control;
            this.FileList.Cursor = System.Windows.Forms.Cursors.Default;
            this.FileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileList.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.FileList.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FileList.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.FileList.Location = new System.Drawing.Point(0, 0);
            // 
            // FileList
            // 
            this.FileList.MasterTemplate.AddNewRowPosition = Telerik.WinControls.UI.SystemRowPosition.Bottom;
            this.FileList.MasterTemplate.AutoGenerateColumns = false;
            gridViewBrowseColumn1.AllowGroup = false;
            gridViewBrowseColumn1.EnableExpressionEditor = false;
            gridViewBrowseColumn1.ExcelExportType = Telerik.WinControls.UI.Export.DisplayFormatType.Text;
            gridViewBrowseColumn1.FieldName = "Name";
            gridViewBrowseColumn1.FormatString = "";
            gridViewBrowseColumn1.HeaderText = "File";
            gridViewBrowseColumn1.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            gridViewBrowseColumn1.MinWidth = 150;
            gridViewBrowseColumn1.Name = "FileNameColumn";
            gridViewBrowseColumn1.Width = 300;
            gridViewImageColumn1.AllowResize = false;
            gridViewImageColumn1.DataType = typeof(int);
            gridViewImageColumn1.EnableExpressionEditor = false;
            gridViewImageColumn1.FieldName = "Progress";
            gridViewImageColumn1.FormatString = "";
            gridViewImageColumn1.HeaderText = "Progress";
            gridViewImageColumn1.MaxWidth = 70;
            gridViewImageColumn1.MinWidth = 70;
            gridViewImageColumn1.Name = "ProgressColumn";
            gridViewImageColumn1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            gridViewImageColumn1.Width = 70;
            gridViewComboBoxColumn1.DataType = typeof(System.Collections.Generic.List<System.Type>);
            gridViewComboBoxColumn1.DisplayMember = null;
            gridViewComboBoxColumn1.EnableExpressionEditor = false;
            gridViewComboBoxColumn1.FieldName = "ObjectType";
            gridViewComboBoxColumn1.FormatString = "";
            gridViewComboBoxColumn1.HeaderText = "Output Type";
            gridViewComboBoxColumn1.MinWidth = 100;
            gridViewComboBoxColumn1.Name = "OutputTypeColumn";
            gridViewComboBoxColumn1.ValueMember = null;
            gridViewComboBoxColumn1.Width = 250;
            gridViewTextBoxColumn1.EnableExpressionEditor = false;
            gridViewTextBoxColumn1.FieldName = "ObjectCount";
            gridViewTextBoxColumn1.FormatString = "";
            gridViewTextBoxColumn1.HeaderText = "Output Count";
            gridViewTextBoxColumn1.Name = "CountColumn";
            gridViewTextBoxColumn1.SortOrder = Telerik.WinControls.UI.RadSortOrder.Ascending;
            gridViewTextBoxColumn1.Width = 150;
            gridViewCommandColumn1.EnableExpressionEditor = false;
            gridViewCommandColumn1.FieldName = "Parse";
            gridViewCommandColumn1.FormatString = "";
            gridViewCommandColumn1.HeaderText = "Parse";
            gridViewCommandColumn1.Name = "ParseColumn";
            this.FileList.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewBrowseColumn1,
            gridViewImageColumn1,
            gridViewComboBoxColumn1,
            gridViewTextBoxColumn1,
            gridViewCommandColumn1});
            this.FileList.MasterTemplate.EnableAlternatingRowColor = true;
            sortDescriptor1.PropertyName = "CountColumn";
            this.FileList.MasterTemplate.SortDescriptors.AddRange(new Telerik.WinControls.Data.SortDescriptor[] {
            sortDescriptor1});
            this.FileList.Name = "FileList";
            this.FileList.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.FileList.Size = new System.Drawing.Size(800, 363);
            this.FileList.TabIndex = 0;
            this.FileList.Text = "FileList";
          // 
            // ButtonImport
            // 
            this.ButtonImport.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonImport.Location = new System.Drawing.Point(0, 318);
            this.ButtonImport.Name = "ButtonImport";
            this.ButtonImport.Size = new System.Drawing.Size(800, 45);
            this.ButtonImport.TabIndex = 1;
            this.ButtonImport.Text = "Import";
            // 
            // ImportFilesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ButtonImport);
            this.Controls.Add(this.FileList);
            this.DoubleBuffered = true;
            this.Name = "ImportFilesView";
            this.Size = new System.Drawing.Size(800, 363);
            ((System.ComponentModel.ISupportInitialize)(this.FileList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonImport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.Themes.TelerikMetroBlueTheme TelerikMetroBlueTheme;
        private Telerik.WinControls.UI.RadGridView FileList;
        private Telerik.WinControls.UI.RadButton ButtonImport;
    }
}
