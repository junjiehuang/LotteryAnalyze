using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EditorUIFramework.Docking;
using EditorUIFramework.Framework;

namespace AirStudio
{
    public partial class MainWindow : Form, IMainWindow
    {
#region Fields

        Dictionary<string, DockContent> mDockViewMap = null;
        DeserializeDockContent m_deserializeDockContent = null;
        CommandWindow mCommandWindow = null;
        GameModeView mGameModeView = null;
        SceneEditView mSceneEditView = null;
        HierarchyWindow mHierarchyWindow = null;
        PropertyWindow mPropertyWindow = null;
        ResourceBrowser mResourceBrowser = null;

#endregion Fields

        public MainWindow()
        {
            EditorRoot.GetInstance().MainUI = this;

            InitializeComponent();
            mDockViewMap = new Dictionary<string, DockContent>();
            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            dockPanelMain.DocumentStyle = DocumentStyle.DockingWindow;
            CreateAllWindows();
        }

        private void DestroyAllWindows()
        {
            UnRegistorDockView(mCommandWindow);
            UnRegistorDockView(mGameModeView);            
            UnRegistorDockView(mSceneEditView);            
            UnRegistorDockView(mHierarchyWindow);            
            UnRegistorDockView(mPropertyWindow);
            UnRegistorDockView(mResourceBrowser);
            mCommandWindow = null;
            mGameModeView = null;
            mSceneEditView = null;
            mHierarchyWindow = null;
            mPropertyWindow = null;
            mResourceBrowser = null;
        }

        public void CreateAllWindows()
        {
            mCommandWindow = new CommandWindow();            
            mGameModeView = new GameModeView();            
            mSceneEditView = new SceneEditView();
            mHierarchyWindow = new HierarchyWindow();            
            mPropertyWindow = new PropertyWindow();            
            mResourceBrowser = new ResourceBrowser();
            FastRegistorView(mCommandWindow);
            FastRegistorView(mGameModeView);
            FastRegistorView(mSceneEditView);
            FastRegistorView(mHierarchyWindow);
            FastRegistorView(mPropertyWindow);
            FastRegistorView(mResourceBrowser);
        }

#region Inherit Interfaces

        public void RegistorDockView(string name, DockContent view)
        {
            if (mDockViewMap.ContainsKey(name))
            {
                if (mDockViewMap[name] != null && mDockViewMap[name] != view)
                    throw new Exception("DockContent view " + name + " has exist" );
                mDockViewMap[name] = view;
            }
            else
                mDockViewMap.Add(name, view);
        }
        public void UnRegistorDockView(string name)
        {
            if (mDockViewMap.ContainsKey(name))
                mDockViewMap.Remove(name);
        }
        public void UnRegistorDockView(DockContent view)
        {
            foreach (string key in mDockViewMap.Keys)
            {
                if (mDockViewMap[key] == view)
                {
                    mDockViewMap.Remove(key);
                    return;
                }
            }            
        }
        public DockContent FindView(string name)
        {
            if (mDockViewMap.ContainsKey(name))
                return mDockViewMap[name];
            return null;
        }

#endregion Inherit Interfaces

#region Member Utils

        private IDockContent GetContentFromPersistString(string persistString)
        {
            DockContent view = FindView(persistString);
            return view;
        }
        
        void FastRegistorView(DockContent view)
        {
            if (view != null)
                RegistorDockView(view.GetType().ToString(), view);
        }
        private void RecoverToDefaultLayout()
        {            
            dockPanelMain.SuspendLayout(true);
      
            mSceneEditView.Show(dockPanelMain, DockState.Document);
            //mGameModeView.Show(mSceneEditView.Pane, DockAlignment.Right, 0.5);
            mGameModeView.Show(mSceneEditView.Pane, mSceneEditView);
            mCommandWindow.Show(dockPanelMain, DockState.DockBottom);            
            mPropertyWindow.Show(dockPanelMain, DockState.DockRight);
            mHierarchyWindow.Show(dockPanelMain, DockState.DockLeft);
            mResourceBrowser.Show(mCommandWindow.Pane, mCommandWindow);
            
            dockPanelMain.ResumeLayout(true, true);
        }
        
#endregion Member Utils

#region Event Triggers

        private void MainWindow_Load(object sender, System.EventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (File.Exists(configFile))
            {
                dockPanelMain.LoadFromXml(configFile, m_deserializeDockContent);
                for (int index = dockPanelMain.Contents.Count - 1; index >= 0; index--)
                {
                    if (dockPanelMain.Contents[index] is DockContent)
                    {
                        DockContent content = (DockContent)dockPanelMain.Contents[index];
                        FastRegistorView(content);
                    }
                }
            }
            else
                RecoverToDefaultLayout();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            dockPanelMain.SaveAsXml(configFile);

            dockPanelMain.SuspendLayout(true);
            DestroyAllWindows();
            dockPanelMain.ResumeLayout(true, true);
        }

#endregion Event Triggers
    }
}
