using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHistoryCopy.Model
{
    public class FileDetail : IEquatable<FileDetail>
    {
        public string FileName{get;set;}
        public string FullFileName { get; set; }
        public long FileLength { get; set; }
        public DateTime LastWriteTime { get; set; }
        public DateTime TimeStamp { get; set; }
        // 效能優化：預存資料夾路徑，避免 Equals 裡一直切字串
        private string _directoryPath;
        public string DirectoryPath
        {
            get => _directoryPath ?? (_directoryPath = Path.GetDirectoryName(FullFileName));
            set => _directoryPath = value;
        }

        // 核心邏輯：定義什麼叫「同一個檔案的各個版本」
        public bool Equals(FileDetail other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(DirectoryPath, other.DirectoryPath, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(FileName, other.FileName, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj) => Equals(obj as FileDetail);

        public override int GetHashCode()
        {
            // 這裡必須跟 Equals 的邏輯完全對應！
            unchecked
            {
                int hash = 17;
                var comparer = StringComparer.OrdinalIgnoreCase;
                hash = hash * 23 + (DirectoryPath != null ? comparer.GetHashCode(DirectoryPath) : 0);
                hash = hash * 23 + (FileName != null ? comparer.GetHashCode(FileName) : 0);
                return hash;
            }
        }
    }
}
