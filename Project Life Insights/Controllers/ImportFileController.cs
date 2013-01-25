using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLifeInsights.MVC;
using ProjectLifeInsights.Models;
using System.IO;
using ProjectLifeInsights.Models.Collection;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;

namespace ProjectLifeInsights.Controllers
{
    /// <summary>
    /// Delegate for progress
    /// </summary>
    /// <param name="file">File that was processed</param>
    /// <param name="result">The resulting types</param>
    /// <param name="userState">The user token</param>
    public delegate void ProgressFileChanged(String file, Dictionary<Type, IModel> result, Object userState);

    public class ImportFileController 
    {
        /// <summary>
        /// Event invoked when a file in processed.
        /// </summary>
        public event ProgressFileChanged OnProgress = delegate { };

        /// <summary>
        /// Processes a file list
        /// </summary>
        /// <param name="fileList">List of file paths to process</param>
        /// <returns>Dictionairy with all parse results</returns>
        internal Dictionary<String, Dictionary<Type, IModel>> ProcessFileList(String[] fileList)
        {
            var task = ProcessFileListAsync(fileList, CancellationToken.None);
            return task.Result;
        }

        /// <summary>
        /// Processes a file list (async)
        /// </summary>
        /// <param name="fileList">List of file paths to process</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="userState">Call token</param>
        /// <returns>Process task</returns>
        internal Task<Dictionary<String, Dictionary<Type, IModel>>> ProcessFileListAsync(String[] fileList, CancellationToken token, Object userState = null) 
        {
            return Task<Dictionary<String, Dictionary<Type, IModel>>>.Factory.StartNew(() =>
            {
                Dictionary<String, Dictionary<Type, IModel>> processed = new Dictionary<String, Dictionary<Type, IModel>>();
                List<Task> tasks = new List<Task>();
                foreach (var file in fileList)
                {
                    var currentFile = file.Clone() as String;
                    tasks.Add(ProcessFileAsync(currentFile, token, userState).ContinueWith((t) => { processed.Add(currentFile, t.Result); }));
                }

                Task.WaitAll(tasks.ToArray());
                return processed;
            });
        }

        /// <summary>
        /// Processes a file
        /// </summary>
        /// <param name="file">File path to process</param>
        /// <returns>All parsing possibilities for the file</returns>
        internal Dictionary<Type, IModel> ProcessFile(String file)
        {
            var task = ProcessFileAsync(file, CancellationToken.None);
            return task.Result;
        }

        /// <summary>
        /// Processes a file (async)
        /// </summary>
        /// <param name="file">File path to process</param>
        /// <param name="userState">Call token</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        internal Task<Dictionary<Type, IModel>> ProcessFileAsync(String file, CancellationToken token, Object userState = null)
        {
            return Task<Dictionary<Type, IModel>>.Factory.StartNew(() =>
            {
                var process = new Dictionary<Type, IModel>();
                var data = String.Empty;
                using (StreamReader stream = File.OpenText(file))
                    data = stream.ReadToEnd();

                TransactionList transactionList;
                if (TransactionList.TryParse(data, out transactionList))
                    process.Add(transactionList.GetType(), transactionList);

                // Cancel
                token.ThrowIfCancellationRequested();

                PhoneCallList phoneCallList;
                if (PhoneCallList.TryParse(data, out phoneCallList))
                    process.Add(phoneCallList.GetType(), phoneCallList);

                // Cancel
                token.ThrowIfCancellationRequested();

                // Progress invoke
                OnProgress.Invoke(file, process, userState);

                Console.WriteLine(file);
                foreach (var type in process) {
                    Console.WriteLine("\t" + type.Key.FullName);
                    Console.WriteLine("\t\t" + type.Value);
                }

                return process;
            });
        }

    }
}
