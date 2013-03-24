using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Threading;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Common;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service
{
    public class FileService
    {
        private WebClient _webClient;

        public delegate void FileLoadedHandler(int i);
        public event FileLoadedHandler FileLoadedEvent;

        public delegate void ProgressHandler(int progress);
        public event ProgressHandler ProgressEvent;

        private IEnumerator<Item> _items;

        public FileService()
        {
            _webClient = new WebClient();
            _webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(OnFileDownloaded);
            _webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadProgressChanged);
            _items = null;
        }

        private void OnFileDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            FileLoadedEvent(1);
            downloadNext();
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressEvent(e.ProgressPercentage);
        }

        private void downloadNext()
        {
            if (_items == null)
                return;

            if (_items.MoveNext())
            {
                LoadFileFrom(_items.Current.Url, _items.Current.Path, _items.Current.User, _items.Current.Password);
            }
        }

        private void resetClient(string user, string password)
        {
            if (_webClient != null)
                _webClient.CancelAsync();

            _webClient = new WebClient();
            if (user != string.Empty && password != string.Empty)
            {
                _webClient.Credentials = new NetworkCredential(user, password);
            }
            _webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(OnFileDownloaded);
            _webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadProgressChanged);
        }

        public void LoadFileFrom(string url, string destination, string user, string password)
        {
            resetClient(user, password);
            _webClient.DownloadFileAsync(new Uri(url), destination);
        }

        public void LoadFilesFrom(IList<Item> items)
        {
            _items = items.GetEnumerator();
            downloadNext();
        }

        public void CancelDownloads()
        {
            _webClient.CancelAsync();
        }

        public string CreatePath(string directory, string fileName)
        {
            return Path.Combine(directory, fileName);
        }
    }
}
