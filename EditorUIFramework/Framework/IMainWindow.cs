using System;
using System.Collections.Generic;
using System.Text;

namespace EditorUIFramework.Framework
{
    public interface IMainWindow
    {
#region Interface functions
        void RegistorDockView(string name, Docking.DockContent view);
        void UnRegistorDockView(string name);
        void UnRegistorDockView(Docking.DockContent view);
        Docking.DockContent FindView(string name);
#endregion Interface functions
    }
}
