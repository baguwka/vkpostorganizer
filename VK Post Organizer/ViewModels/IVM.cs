using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vk.ViewModels {
   public interface IVM {
      void OnLoad();
      void OnClosing();
      void OnClosed();
   }
}
