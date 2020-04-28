using System.Text;
using System.Threading.Tasks;

namespace FlexibleDBMS
{
    public interface IReadable
    {
        Task ReadAsync(string filePath);
        Task ReadAsync(string filePath, Encoding encoding);
        Task ReadAsync(string filePath, int maxElementsInDictionary);
        Task ReadAsync(string filePath, Encoding encoding, int maxElementsInDictionary);

        Task ReadConfigAsync(string filePath);
        

        delegate void WorkFinished(object sender, BoolEventArgs e);
        delegate void InfoMessage(object sender, TextEventArgs e);
    }

}
