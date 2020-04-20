using System.Text;
using System.Threading.Tasks;

namespace AutoAnalysis
{
    public interface IReadable
    {
        Task Read(string filePath);
        Task Read(string filePath, Encoding encoding);
        Task Read(string filePath, int maxElementsInDictionary);
        Task Read(string filePath, Encoding encoding, int maxElementsInDictionary);

        Task ReadConfigAsync(string filePath);

        delegate void WorkFinished(object sender, BoolEventArgs e);
        delegate void InfoMessage(object sender, TextEventArgs e);
    }

}
