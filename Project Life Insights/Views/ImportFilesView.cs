using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProjectLifeInsights.Models.Collection;
using System.IO;
using Telerik.WinControls.UI;
using ProjectLifeInsights.Controllers;
using System.Threading;
using System.Collections;
using ProjectLifeInsights.Models;

namespace ProjectLifeInsights.Views
{
    public partial class ImportFilesView : UserControl
    {
        private ImportFileController _controller;
        private Dictionary<String, Dictionary<Type, MVC.IModel>> _processedData;

        /// <summary>
        /// 
        /// </summary>
        public ImportFilesView()
        {
            InitializeComponent();

            _controller = new ImportFileController();
            _processedData = new Dictionary<String, Dictionary<Type, MVC.IModel>>();

            this.FileList.CommandCellClick += new CommandCellClickEventHandler(FileList_CommandCellClick);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileList_CommandCellClick(object sender, EventArgs e)
        {
            var command = sender as GridCommandCellElement;
            var eventargs = e as GridViewCellEventArgs;
            var cel = this.FileList.MasterTemplate.Columns[eventargs.ColumnIndex];

            switch(cel.FieldName)  {
                case "Parse":
                    if (eventargs.Row.Cells[0].Value != null)
                    {
                        var file = eventargs.Row.Cells[0].Value as String;
                        var output = eventargs.Row.Cells[2].Value;
                        //this.FileList.DataSource
                        if (File.Exists(file))
                        {
                            //_controller.OnProgress += new ProgressFileChanged(_controller_OnProgress);
                            _controller.ProcessFileAsync(file, CancellationToken.None, eventargs.RowIndex)
                                .ContinueWith((t) => { 
                                    
                                });
                        }
                    }
                    break;
            
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        /// <param name="userState"></param>
        private void _controller_OnProgress(String file, Dictionary<Type, MVC.IModel> result, object userState)
        {
            _processedData.Add(file, result);

        }
        
    }

}