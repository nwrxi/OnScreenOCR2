using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OnScreenOCR
{
    public interface IOcrHandler
    {
        Task<string> RecognizeImage(byte[] imageArr);
    }
}