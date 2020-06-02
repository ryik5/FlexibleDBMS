using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlexibleDBMS
{
    public interface IReadable
    {
        public IList<string> Text { get; }

        Task<IList<string>> ReadAsync(string filePath);
        Task<IList<string>> ReadAsync(string filePath, Encoding encoding);
        Task<IList<string>> ReadAsync(string filePath, int maxElementsInDictionary);
        Task<IList<string>> ReadAsync(string filePath, Encoding encoding, int maxElementsInDictionary);

        IList<string> Read(string filePath);
        IList<string> Read(string filePath, Encoding encoding);
        IList<string> Read(string filePath,  int maxElementsInDictionary);
        IList<string> Read(string filePath, Encoding encoding, int maxElementsInDictionary);


        delegate void WorkFinished(object sender, BoolEventArgs e);
        delegate void InfoMessage(object sender, TextEventArgs e);
    }

}
