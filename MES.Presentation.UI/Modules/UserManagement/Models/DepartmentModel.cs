using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Presentation.UI.Modules.UserManagement.Models
{
    public class DepartmentModel
    {
        public required string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
