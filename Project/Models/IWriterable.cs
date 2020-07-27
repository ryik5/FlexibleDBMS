using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlexibleDBMS
{
    public interface IWriterable
    {
      //  Task Delete(string filePath);
        Task Write(string filePath, string content);
        Task Write(string filePath, IList<string> content);
        Task Write(string filePath, string content, Encoding encoding);
        Task Write(string filePath, IList<string> content, Encoding encoding);
        void Write(string filePath, ConfigFull<ConfigAbstract> config);
    }
}