using System.IO;
using System.Threading.Tasks;

namespace TaskCat.Common.WebApi
{
    public interface ICompressor
    {
        string EncodingType { get; }
        Task Compress(Stream source, Stream destination);
        Task Decompress(Stream source, Stream destination);
    }
}