using everybody_codes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraSearch.Services.Interfaces {
    public interface ICameraService {
        Task<List<Camera>> GetAllAsync();
    }
}
