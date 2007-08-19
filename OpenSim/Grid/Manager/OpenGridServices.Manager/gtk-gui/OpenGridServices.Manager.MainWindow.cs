// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace OpenGridServices.Manager {
    
    
    public partial class MainWindow {
        
        private Gtk.Action Grid;
        
        private Gtk.Action User;
        
        private Gtk.Action Asset;
        
        private Gtk.Action Region;
        
        private Gtk.Action Services;
        
        private Gtk.Action ConnectToGridserver;
        
        private Gtk.Action RestartWholeGrid;
        
        private Gtk.Action ShutdownWholeGrid;
        
        private Gtk.Action ExitGridManager;
        
        private Gtk.Action ConnectToUserserver;
        
        private Gtk.Action AccountManagment;
        
        private Gtk.Action GlobalNotice;
        
        private Gtk.Action DisableAllLogins;
        
        private Gtk.Action DisableNonGodUsersOnly;
        
        private Gtk.Action ShutdownUserServer;
        
        private Gtk.Action ShutdownGridserverOnly;
        
        private Gtk.Action RestartGridserverOnly;
        
        private Gtk.Action DefaultLocalGridUserserver;
        
        private Gtk.Action CustomUserserver;
        
        private Gtk.Action RemoteGridDefaultUserserver;
        
        private Gtk.Action DisconnectFromGridServer;
        
        private Gtk.Action UploadAsset;
        
        private Gtk.Action AssetManagement;
        
        private Gtk.Action ConnectToAssetServer;
        
        private Gtk.Action ConnectToDefaultAssetServerForGrid;
        
        private Gtk.Action DefaultForLocalGrid;
        
        private Gtk.Action DefaultForRemoteGrid;
        
        private Gtk.Action CustomAssetServer;
        
        private Gtk.VBox vbox1;
        
        private Gtk.MenuBar menubar2;
        
        private Gtk.HBox hbox1;
        
        private Gtk.ScrolledWindow scrolledwindow1;
        
        private Gtk.DrawingArea drawingarea1;
        
        private Gtk.TreeView treeview1;
        
        private Gtk.Statusbar statusbar1;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize();
            // Widget OpenGridServices.Manager.MainWindow
            Gtk.UIManager w1 = new Gtk.UIManager();
            Gtk.ActionGroup w2 = new Gtk.ActionGroup("Default");
            this.Grid = new Gtk.Action("Grid", Mono.Unix.Catalog.GetString("Grid"), null, null);
            this.Grid.HideIfEmpty = false;
            this.Grid.ShortLabel = Mono.Unix.Catalog.GetString("Grid");
            w2.Add(this.Grid, "<Alt><Mod2>g");
            this.User = new Gtk.Action("User", Mono.Unix.Catalog.GetString("User"), null, null);
            this.User.HideIfEmpty = false;
            this.User.ShortLabel = Mono.Unix.Catalog.GetString("User");
            w2.Add(this.User, null);
            this.Asset = new Gtk.Action("Asset", Mono.Unix.Catalog.GetString("Asset"), null, null);
            this.Asset.HideIfEmpty = false;
            this.Asset.ShortLabel = Mono.Unix.Catalog.GetString("Asset");
            w2.Add(this.Asset, null);
            this.Region = new Gtk.Action("Region", Mono.Unix.Catalog.GetString("Region"), null, null);
            this.Region.ShortLabel = Mono.Unix.Catalog.GetString("Region");
            w2.Add(this.Region, null);
            this.Services = new Gtk.Action("Services", Mono.Unix.Catalog.GetString("Services"), null, null);
            this.Services.ShortLabel = Mono.Unix.Catalog.GetString("Services");
            w2.Add(this.Services, null);
            this.ConnectToGridserver = new Gtk.Action("ConnectToGridserver", Mono.Unix.Catalog.GetString("Connect to gridserver..."), null, "gtk-connect");
            this.ConnectToGridserver.HideIfEmpty = false;
            this.ConnectToGridserver.ShortLabel = Mono.Unix.Catalog.GetString("Connect to gridserver");
            w2.Add(this.ConnectToGridserver, null);
            this.RestartWholeGrid = new Gtk.Action("RestartWholeGrid", Mono.Unix.Catalog.GetString("Restart whole grid"), null, "gtk-refresh");
            this.RestartWholeGrid.ShortLabel = Mono.Unix.Catalog.GetString("Restart whole grid");
            w2.Add(this.RestartWholeGrid, null);
            this.ShutdownWholeGrid = new Gtk.Action("ShutdownWholeGrid", Mono.Unix.Catalog.GetString("Shutdown whole grid"), null, "gtk-stop");
            this.ShutdownWholeGrid.ShortLabel = Mono.Unix.Catalog.GetString("Shutdown whole grid");
            w2.Add(this.ShutdownWholeGrid, null);
            this.ExitGridManager = new Gtk.Action("ExitGridManager", Mono.Unix.Catalog.GetString("Exit grid manager"), null, "gtk-close");
            this.ExitGridManager.ShortLabel = Mono.Unix.Catalog.GetString("Exit grid manager");
            w2.Add(this.ExitGridManager, null);
            this.ConnectToUserserver = new Gtk.Action("ConnectToUserserver", Mono.Unix.Catalog.GetString("Connect to userserver"), null, "gtk-connect");
            this.ConnectToUserserver.ShortLabel = Mono.Unix.Catalog.GetString("Connect to userserver");
            w2.Add(this.ConnectToUserserver, null);
            this.AccountManagment = new Gtk.Action("AccountManagment", Mono.Unix.Catalog.GetString("Account managment"), null, "gtk-properties");
            this.AccountManagment.ShortLabel = Mono.Unix.Catalog.GetString("Account managment");
            w2.Add(this.AccountManagment, null);
            this.GlobalNotice = new Gtk.Action("GlobalNotice", Mono.Unix.Catalog.GetString("Global notice"), null, "gtk-network");
            this.GlobalNotice.ShortLabel = Mono.Unix.Catalog.GetString("Global notice");
            w2.Add(this.GlobalNotice, null);
            this.DisableAllLogins = new Gtk.Action("DisableAllLogins", Mono.Unix.Catalog.GetString("Disable all logins"), null, "gtk-no");
            this.DisableAllLogins.ShortLabel = Mono.Unix.Catalog.GetString("Disable all logins");
            w2.Add(this.DisableAllLogins, null);
            this.DisableNonGodUsersOnly = new Gtk.Action("DisableNonGodUsersOnly", Mono.Unix.Catalog.GetString("Disable non-god users only"), null, "gtk-no");
            this.DisableNonGodUsersOnly.ShortLabel = Mono.Unix.Catalog.GetString("Disable non-god users only");
            w2.Add(this.DisableNonGodUsersOnly, null);
            this.ShutdownUserServer = new Gtk.Action("ShutdownUserServer", Mono.Unix.Catalog.GetString("Shutdown user server"), null, "gtk-stop");
            this.ShutdownUserServer.ShortLabel = Mono.Unix.Catalog.GetString("Shutdown user server");
            w2.Add(this.ShutdownUserServer, null);
            this.ShutdownGridserverOnly = new Gtk.Action("ShutdownGridserverOnly", Mono.Unix.Catalog.GetString("Shutdown gridserver only"), null, "gtk-stop");
            this.ShutdownGridserverOnly.ShortLabel = Mono.Unix.Catalog.GetString("Shutdown gridserver only");
            w2.Add(this.ShutdownGridserverOnly, null);
            this.RestartGridserverOnly = new Gtk.Action("RestartGridserverOnly", Mono.Unix.Catalog.GetString("Restart gridserver only"), null, "gtk-refresh");
            this.RestartGridserverOnly.ShortLabel = Mono.Unix.Catalog.GetString("Restart gridserver only");
            w2.Add(this.RestartGridserverOnly, null);
            this.DefaultLocalGridUserserver = new Gtk.Action("DefaultLocalGridUserserver", Mono.Unix.Catalog.GetString("Default local grid userserver"), null, null);
            this.DefaultLocalGridUserserver.ShortLabel = Mono.Unix.Catalog.GetString("Default local grid userserver");
            w2.Add(this.DefaultLocalGridUserserver, null);
            this.CustomUserserver = new Gtk.Action("CustomUserserver", Mono.Unix.Catalog.GetString("Custom userserver..."), null, null);
            this.CustomUserserver.ShortLabel = Mono.Unix.Catalog.GetString("Custom userserver");
            w2.Add(this.CustomUserserver, null);
            this.RemoteGridDefaultUserserver = new Gtk.Action("RemoteGridDefaultUserserver", Mono.Unix.Catalog.GetString("Remote grid default userserver..."), null, null);
            this.RemoteGridDefaultUserserver.ShortLabel = Mono.Unix.Catalog.GetString("Remote grid default userserver");
            w2.Add(this.RemoteGridDefaultUserserver, null);
            this.DisconnectFromGridServer = new Gtk.Action("DisconnectFromGridServer", Mono.Unix.Catalog.GetString("Disconnect from grid server"), null, "gtk-disconnect");
            this.DisconnectFromGridServer.ShortLabel = Mono.Unix.Catalog.GetString("Disconnect from grid server");
            this.DisconnectFromGridServer.Visible = false;
            w2.Add(this.DisconnectFromGridServer, null);
            this.UploadAsset = new Gtk.Action("UploadAsset", Mono.Unix.Catalog.GetString("Upload asset"), null, null);
            this.UploadAsset.ShortLabel = Mono.Unix.Catalog.GetString("Upload asset");
            w2.Add(this.UploadAsset, null);
            this.AssetManagement = new Gtk.Action("AssetManagement", Mono.Unix.Catalog.GetString("Asset management"), null, null);
            this.AssetManagement.ShortLabel = Mono.Unix.Catalog.GetString("Asset management");
            w2.Add(this.AssetManagement, null);
            this.ConnectToAssetServer = new Gtk.Action("ConnectToAssetServer", Mono.Unix.Catalog.GetString("Connect to asset server"), null, null);
            this.ConnectToAssetServer.ShortLabel = Mono.Unix.Catalog.GetString("Connect to asset server");
            w2.Add(this.ConnectToAssetServer, null);
            this.ConnectToDefaultAssetServerForGrid = new Gtk.Action("ConnectToDefaultAssetServerForGrid", Mono.Unix.Catalog.GetString("Connect to default asset server for grid"), null, null);
            this.ConnectToDefaultAssetServerForGrid.ShortLabel = Mono.Unix.Catalog.GetString("Connect to default asset server for grid");
            w2.Add(this.ConnectToDefaultAssetServerForGrid, null);
            this.DefaultForLocalGrid = new Gtk.Action("DefaultForLocalGrid", Mono.Unix.Catalog.GetString("Default for local grid"), null, null);
            this.DefaultForLocalGrid.ShortLabel = Mono.Unix.Catalog.GetString("Default for local grid");
            w2.Add(this.DefaultForLocalGrid, null);
            this.DefaultForRemoteGrid = new Gtk.Action("DefaultForRemoteGrid", Mono.Unix.Catalog.GetString("Default for remote grid..."), null, null);
            this.DefaultForRemoteGrid.ShortLabel = Mono.Unix.Catalog.GetString("Default for remote grid...");
            w2.Add(this.DefaultForRemoteGrid, null);
            this.CustomAssetServer = new Gtk.Action("CustomAssetServer", Mono.Unix.Catalog.GetString("Custom asset server..."), null, null);
            this.CustomAssetServer.ShortLabel = Mono.Unix.Catalog.GetString("Custom asset server...");
            w2.Add(this.CustomAssetServer, null);
            w1.InsertActionGroup(w2, 0);
            this.AddAccelGroup(w1.AccelGroup);
            this.WidthRequest = 800;
            this.HeightRequest = 600;
            this.Name = "OpenGridServices.Manager.MainWindow";
            this.Title = Mono.Unix.Catalog.GetString("Open Grid Services Manager");
            this.Icon = Gtk.IconTheme.Default.LoadIcon("gtk-network", 48, 0);
            // Container child OpenGridServices.Manager.MainWindow.Gtk.Container+ContainerChild
            this.vbox1 = new Gtk.VBox();
            this.vbox1.Name = "vbox1";
            // Container child vbox1.Gtk.Box+BoxChild
            w1.AddUiFromString("<ui><menubar name='menubar2'><menu action='Grid'><menuitem action='ConnectToGridserver'/><menuitem action='DisconnectFromGridServer'/><separator/><menuitem action='RestartWholeGrid'/><menuitem action='RestartGridserverOnly'/><separator/><menuitem action='ShutdownWholeGrid'/><menuitem action='ShutdownGridserverOnly'/><separator/><menuitem action='ExitGridManager'/></menu><menu action='User'><menu action='ConnectToUserserver'><menuitem action='DefaultLocalGridUserserver'/><menuitem action='CustomUserserver'/><menuitem action='RemoteGridDefaultUserserver'/></menu><separator/><menuitem action='AccountManagment'/><menuitem action='GlobalNotice'/><separator/><menuitem action='DisableAllLogins'/><menuitem action='DisableNonGodUsersOnly'/><separator/><menuitem action='ShutdownUserServer'/></menu><menu action='Asset'><menuitem action='UploadAsset'/><menuitem action='AssetManagement'/><menu action='ConnectToAssetServer'><menuitem action='DefaultForLocalGrid'/><menuitem action='DefaultForRemoteGrid'/><menuitem action='CustomAssetServer'/></menu></menu><menu action='Region'/><menu action='Services'/></menubar></ui>");
            this.menubar2 = ((Gtk.MenuBar)(w1.GetWidget("/menubar2")));
            this.menubar2.HeightRequest = 25;
            this.menubar2.Name = "menubar2";
            this.vbox1.Add(this.menubar2);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.vbox1[this.menubar2]));
            w3.Position = 0;
            w3.Expand = false;
            w3.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            // Container child hbox1.Gtk.Box+BoxChild
            this.scrolledwindow1 = new Gtk.ScrolledWindow();
            this.scrolledwindow1.CanFocus = true;
            this.scrolledwindow1.Name = "scrolledwindow1";
            this.scrolledwindow1.VscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow1.HscrollbarPolicy = ((Gtk.PolicyType)(1));
            // Container child scrolledwindow1.Gtk.Container+ContainerChild
            Gtk.Viewport w4 = new Gtk.Viewport();
            w4.Name = "GtkViewport";
            w4.ShadowType = ((Gtk.ShadowType)(0));
            // Container child GtkViewport.Gtk.Container+ContainerChild
            this.drawingarea1 = new Gtk.DrawingArea();
            this.drawingarea1.Name = "drawingarea1";
            w4.Add(this.drawingarea1);
            this.scrolledwindow1.Add(w4);
            this.hbox1.Add(this.scrolledwindow1);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.hbox1[this.scrolledwindow1]));
            w7.Position = 1;
            // Container child hbox1.Gtk.Box+BoxChild
            this.treeview1 = new Gtk.TreeView();
            this.treeview1.CanFocus = true;
            this.treeview1.Name = "treeview1";
            this.hbox1.Add(this.treeview1);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(this.hbox1[this.treeview1]));
            w8.Position = 2;
            this.vbox1.Add(this.hbox1);
            Gtk.Box.BoxChild w9 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox1]));
            w9.Position = 1;
            // Container child vbox1.Gtk.Box+BoxChild
            this.statusbar1 = new Gtk.Statusbar();
            this.statusbar1.Name = "statusbar1";
            this.statusbar1.Spacing = 5;
            this.vbox1.Add(this.statusbar1);
            Gtk.Box.BoxChild w10 = ((Gtk.Box.BoxChild)(this.vbox1[this.statusbar1]));
            w10.PackType = ((Gtk.PackType)(1));
            w10.Position = 2;
            w10.Expand = false;
            w10.Fill = false;
            this.Add(this.vbox1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 800;
            this.DefaultHeight = 800;
            this.Show();
            this.DeleteEvent += new Gtk.DeleteEventHandler(this.OnDeleteEvent);
            this.ConnectToGridserver.Activated += new System.EventHandler(this.ConnectToGridServerMenu);
            this.ExitGridManager.Activated += new System.EventHandler(this.QuitMenu);
            this.ShutdownGridserverOnly.Activated += new System.EventHandler(this.ShutdownGridserverMenu);
            this.RestartGridserverOnly.Activated += new System.EventHandler(this.RestartGridserverMenu);
            this.DisconnectFromGridServer.Activated += new System.EventHandler(this.DisconnectGridServerMenu);
        }
    }
}
