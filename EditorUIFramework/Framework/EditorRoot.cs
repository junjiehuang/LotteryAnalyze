using System;
using System.Collections.Generic;
using System.Text;

namespace EditorUIFramework.Framework
{
    public class EditorRoot
    {
#region Ctor

        static EditorRoot sInst = null;
        public static EditorRoot GetInstance()
        {
            if (sInst == null)
                sInst = new EditorRoot();
            return sInst;
        }

        private EditorRoot()
        {
        }

#endregion Ctor


#region Fields

        IMainWindow mMainWindow = null;
        public IMainWindow MainUI
        {
            get { return mMainWindow; }
            set 
            {
                if (mMainWindow != null && value != null)
                    throw new Exception("There is a MainWindow Exist!");
                mMainWindow = value; 
            }
        }

#endregion Fields


#region Util Functions
            


#endregion Util Functions
    }
}
