using everybody_codes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraSearch.Repository.Interfaces {
    public interface ICameraRepository {
        Task<List<Camera>> GetAllAsync();
    }
}
