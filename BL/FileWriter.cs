using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FlexibleDBMS
{

    public class FileWriter : IWriterable
    {

        public int importedRows = 0;
        public IList<string> list;

        public delegate void WriteFinished(object sender, BoolEventArgs e);

        public event WriteFinished EvntWriteFinished;

        public delegate void InfoMessage(object sender, TextEventArgs e);
        public event InfoMessage EvntInfoMessage;

        public async Task Write(string filePath, IList<string> content, Encoding encoding)
        {
            const int DefaultBufferSize = 4096;
            const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, DefaultBufferSize, DefaultOptions))
            {
                using (var writer = new StreamWriter(stream, encoding))
                {
                    foreach (var line in content)
                    {
                        await writer.WriteLineAsync(line);
                    }

                    await writer.FlushAsync();
                }
                //   await stream.FlushAsync();
            }

            EvntWriteFinished?.Invoke(this, new BoolEventArgs(true));//last part of the collection
        }

        public async Task Write(string filePath, string content, Encoding encoding)
        {
            const int DefaultBufferSize = 4096;
            const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, DefaultBufferSize, DefaultOptions))
            {
                using (var writer = new StreamWriter(stream, encoding))
                {
                    await writer.WriteAsync(content);

                    await writer.FlushAsync();
                }
                //  await stream.FlushAsync();
            }

            EvntWriteFinished?.Invoke(this, new BoolEventArgs(true));//last part of the collection
        }

        public async Task Write(string filePath, IList<string> content)
        {
            Encoding _encoding = Encoding.GetEncoding(1251);

            await Write(filePath, content, _encoding);
        }

        public async Task Write(string filePath, string content)
        {
            Encoding _encoding = Encoding.GetEncoding(1251);

            await Write(filePath, content, _encoding);
        }

        public async Task Write(string filePath, object config)
        {
            EvntInfoMessage?.Invoke(this, new TextEventArgs($"Try to write {nameof(ConfigParameter)}"));
            if (File.Exists(filePath))
            {
                try { File.Delete(filePath); }
                catch (Exception excpt)
                {
                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"{excpt.Message}"));
                }
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    try
                    {
                        EvntInfoMessage?.Invoke(this, new TextEventArgs($"0 {config.GetPropertyValues().AsString()}"));

                        BinaryFormatter formatter = new BinaryFormatter();
                        await Task.Run(() =>
                        formatter.Serialize(fileStream, config)
                        );

                        EvntInfoMessage?.Invoke(this, new TextEventArgs("ConfigApplication was written."));
                    }
                    catch (Exception excpt)
                    {
                        EvntInfoMessage?.Invoke(this, new TextEventArgs($"1 {excpt.ToString()}"));
                    }
                    EvntWriteFinished?.Invoke(this, new BoolEventArgs(true));//last part of the collection

                    // await fs.FlushAsync();
                }
            }
        }
    }
}
