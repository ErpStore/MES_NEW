using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Presentation.UI.Navigation
{
    public interface INavigationService
    {
        void Navigate(AppPage page);
    }
}
