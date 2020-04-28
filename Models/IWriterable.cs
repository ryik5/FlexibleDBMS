using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlexibleDBMS
{
    public interface IWriterable
    {
        Task Write(string filePath, string content);
        Task Write(string filePath, IList<string> content);
        Task Write(string filePath, string content, Encoding encoding);
        Task Write(string filePath, IList<string> content, Encoding encoding);
        Task Write(string filePath, ConfigFullNew<AbstractConfig> config);

        delegate void WorkFinished(object sender, BoolEventArgs e);
        delegate void InfoMessage(object sender, TextEventArgs e);
    }

}
