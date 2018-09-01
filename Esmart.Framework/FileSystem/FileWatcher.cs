using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Esmart.Framework.Messagging;
using System.Configuration;


namespace Esmart.Framework.FileSystem
{
    /// <summary>
    /// 文件监视
    /// </summary>
    public class FileWatcher
    {
        FileSystemWatcher _fsw;

        /// <param name="dir">监视的文件夹</param>
        public FileWatcher(string dir)
        {
            _fsw = new FileSystemWatcher(dir);
            _fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime;
            Init();
        }

        /// <param name="dir">监视的文件夹</param>
        /// <param name="filter">文件夹中文件的后缀名</param>
        public FileWatcher(string dir, string filter)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            _fsw = new FileSystemWatcher(dir, filter);
            Init();
        }

        public void Start()
        {
            _fsw.EnableRaisingEvents = true;
        }

        private void _fsw_Changed(object sender, FileSystemEventArgs e)
        {
           
            SqlFileMessage message = new SqlFileMessage(e.FullPath);
            MessageBus.Instance.Pubish(message);             
        }

        private void _fsw_Delete(object sender, FileSystemEventArgs e)
        {
           
            SqlFileDeleteMessage message = new SqlFileDeleteMessage(e.FullPath);
            MessageBus.Instance.Pubish(message);
        }

        private void _fsw_Rename(object sender, RenamedEventArgs e)
        {
            SqlFileRenameMessage message = new SqlFileRenameMessage(e.OldFullPath, e.FullPath);
            MessageBus.Instance.Pubish(message);
        }

        private void Init()
        {
            _fsw.Changed += _fsw_Changed;
            _fsw.Created += _fsw_Changed;
            _fsw.Renamed += _fsw_Rename;
            _fsw.Deleted += _fsw_Delete;
        }

    }
}
