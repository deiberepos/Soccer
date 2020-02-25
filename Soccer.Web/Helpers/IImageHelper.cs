using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Soccer.Web.Helpers
{
    public interface IImageHelper
    {
        //Devuelve como quedo la imagen guardada, porque se le cambia el nombre
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}

