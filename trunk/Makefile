# Makefile for NyFolder
# =================================================================================

CC = gmcs
CFLAGS = 

# ===================================== Packages ===========================
PKG_GTK = -pkg:gtk-sharp-2.0
PKG_GLADE = -pkg:glade-sharp-2.0
PKG_SQLITE = -r:System.Data.dll -r:Mono.Data.SqliteClient.dll
PKG_NIRY = -r:niry-sharp.dll
PKGS = $(PKG_GTK) $(PKG_GLADE) $(PKG_SQLITE) $(PKG_NIRY)
# ==========================================================================

# ===================================== NyFolder Info ======================
NAME = NyFolder
VERSION = 0.1 Alpha 1
COMPILER = `$(CC) --version`
# ==========================================================================

GUI_DIALOGS =	GUI/Dialogs/About.cs			\
				GUI/Dialogs/AcceptUser.cs		\
				GUI/Dialogs/AddPeer.cs			\
				GUI/Dialogs/Login.cs			\
				GUI/Dialogs/ProxySettings.cs	\
				GUI/Dialogs/RemovePeer.cs		\
				GUI/Dialogs/SetPort.cs

GUI = 	GUI/FolderStore.cs			\
		GUI/FolderViewer.cs			\
		GUI/MenuManager.cs			\
		GUI/NetworkStore.cs			\
		GUI/NetworkViewer.cs		\
		GUI/NotebookViewer.cs		\
		GUI/StockIcons.cs			\
		GUI/TabLabel.cs				\
		GUI/UserPanel.cs			\
		GUI/Window.cs

GUI_GLUE = 	GUI/Glue/Dialogs.cs			\
			GUI/Glue/FolderManager.cs	\
			GUI/Glue/NetworkManager.cs	\
			GUI/Glue/ProtocolManager.cs

GUI_SRC = $(GUI) $(GUI_DIALOGS) $(GUI_GLUE)

DATABASE_SRC =	

PROTOCOL_SRC = 	Protocol/CmdManager.cs			\
				Protocol/DownloadManager.cs		\
				Protocol/FileReceiver.cs		\
				Protocol/FileSender.cs			\
				Protocol/HttpRequest.cs			\
				Protocol/Login.cs				\
				Protocol/MyInfo.cs				\
				Protocol/UploadManager.cs		\
				Protocol/UserInfo.cs			\
				Protocol/XmlRequest.cs

UTILS_SRC	= 	Utils/Paths.cs  		\
				Utils/Plugin.cs  		\
				Utils/PluginManager.cs	\
				Utils/Proxy.cs

GLADE_RES = Glade/AcceptUserDialog.glade		\
			Glade/AddPeerDialog.glade			\
			Glade/ProxySettingsDialog.glade		\
			Glade/RemovePeerDialog.glade		\
			Glade/SetPortDialog.glade

IMG_RES = 	Pixmaps/Channel.png						\
			Pixmaps/Close.png						\
			Pixmaps/Directory.png					\
			Pixmaps/Download.png					\
			Pixmaps/InsecureAuth.png				\
			Pixmaps/Lock.png						\
			Pixmaps/Logout.png						\
			Pixmaps/MyFolder.png					\
			Pixmaps/MyFolderOffline.png				\
			Pixmaps/MyFolderOfflineInsecure.png		\
			Pixmaps/MyFolderOnline.png				\
			Pixmaps/MyFolderOnlineInsecure.png		\
			Pixmaps/Network.png						\
			Pixmaps/NetworkInsecure.png				\
			Pixmaps/NyFolderIcon.png				\
			Pixmaps/NyFolderLogo.png				\
			Pixmaps/NyFolderSmall.png				\
			Pixmaps/Proxy.png						\
			Pixmaps/SecureAuth.png					\
			Pixmaps/Send.png						\
			Pixmaps/StockMyFolder.png				\
			Pixmaps/StockNetwork.png				\
			Pixmaps/FileTypes/FileTypeAc3.png		\
			Pixmaps/FileTypes/FileTypeAiff.png		\
			Pixmaps/FileTypes/FileTypeArj.png		\
			Pixmaps/FileTypes/FileTypeAu.png		\
			Pixmaps/FileTypes/FileTypeBak.png		\
			Pixmaps/FileTypes/FileTypeBin.png		\
			Pixmaps/FileTypes/FileTypeBmp.png		\
			Pixmaps/FileTypes/FileTypeBz2.png		\
			Pixmaps/FileTypes/FileTypeC.png			\
			Pixmaps/FileTypes/FileTypeClass.png		\
			Pixmaps/FileTypes/FileTypeCss.png		\
			Pixmaps/FileTypes/FileTypeDeb.png		\
			Pixmaps/FileTypes/FileTypeDia.png		\
			Pixmaps/FileTypes/FileTypeDoc.png		\
			Pixmaps/FileTypes/FileTypeDvi.png		\
			Pixmaps/FileTypes/FileTypeEps.png		\
			Pixmaps/FileTypes/FileTypeExe.png		\
			Pixmaps/FileTypes/FileTypeGeneric.png	\
			Pixmaps/FileTypes/FileTypeGif.png		\
			Pixmaps/FileTypes/FileTypeGz.png		\
			Pixmaps/FileTypes/FileTypeH.png			\
			Pixmaps/FileTypes/FileTypeJar.png		\
			Pixmaps/FileTypes/FileTypeJava.png		\
			Pixmaps/FileTypes/FileTypeJpg.png		\
			Pixmaps/FileTypes/FileTypeMidi.png		\
			Pixmaps/FileTypes/FileTypeMod.png		\
			Pixmaps/FileTypes/FileTypeMp3.png		\
			Pixmaps/FileTypes/FileTypeOgg.png		\
			Pixmaps/FileTypes/FileTypePbm.png		\
			Pixmaps/FileTypes/FileTypePdf.png		\
			Pixmaps/FileTypes/FileTypePgp.png		\
			Pixmaps/FileTypes/FileTypePhp.png		\
			Pixmaps/FileTypes/FileTypePl.png		\
			Pixmaps/FileTypes/FileTypePng.png		\
			Pixmaps/FileTypes/FileTypePpt.png		\
			Pixmaps/FileTypes/FileTypePs.png		\
			Pixmaps/FileTypes/FileTypePsd.png		\
			Pixmaps/FileTypes/FileTypePy.png		\
			Pixmaps/FileTypes/FileTypePyc.png		\
			Pixmaps/FileTypes/FileTypeRar.png		\
			Pixmaps/FileTypes/FileTypeRb.png		\
			Pixmaps/FileTypes/FileTypeRm.png		\
			Pixmaps/FileTypes/FileTypeRpm.png		\
			Pixmaps/FileTypes/FileTypeRtf.png		\
			Pixmaps/FileTypes/FileTypeSh.png		\
			Pixmaps/FileTypes/FileTypeSql.png		\
			Pixmaps/FileTypes/FileTypeSvg.png		\
			Pixmaps/FileTypes/FileTypeTar.png		\
			Pixmaps/FileTypes/FileTypeTbz.png		\
			Pixmaps/FileTypes/FileTypeTex.png		\
			Pixmaps/FileTypes/FileTypeTga.png		\
			Pixmaps/FileTypes/FileTypeTgz.png		\
			Pixmaps/FileTypes/FileTypeWav.png		\
			Pixmaps/FileTypes/FileTypeXls.png		\
			Pixmaps/FileTypes/FileTypeXml.png		\
			Pixmaps/FileTypes/FileTypeXwd.png		\
			Pixmaps/FileTypes/FileTypeZip.png

# Sources & Resources
SRC = $(GUI_SRC) $(PROTOCOL_SRC) $(UTILS_SRC) $(DATABASE_SRC) Info.cs
RESOURCES = $(IMG_RES) $(GLADE_RES)

NyFolder: NyFolderPlugin Main.cs
	@echo
	@echo "$(NAME) Main Application"
	@echo "======================================================="
	$(CC) $(CFLAGS) $(PKGS) Main.cs -out:NyFolder.exe -r:nyfolder-plugin.dll

NyFolderPlugin: NyFolderInfo $(GUI_SRC) $(RESOURCES)
	@echo
	@echo "$(NAME) Plugin (DLL)"
	@echo "======================================================="
	$(CC) $(CFLAGS) -target:library $(PKGS) $(SRC) $(RESOURCES:%=-resource:%) -out:nyfolder-plugin.dll
	@echo "======================================================="
	@echo "gmcs -target:library MyPlugin.cs -r:nyfolder-plugin.dll"
	@echo "======================================================="
	@echo

NyFolderInfo: Info.cs.in
	@echo
	@echo "Name: $(NAME)"
	@echo "Version: $(VERSION)"
	@echo "Compiler: $(COMPILER)"
	@echo "======================================================="
	sed -e "s|\@version\@|${VERSION}|" 			\
	    -e "s|\@name\@|${NAME}|" 				\
	    -e "s|\@compiler\@|${COMPILER}|" 		\
		 Info.cs.in > Info.cs
	@echo "======================================================="
	@echo

clean: 
	rm -f *.exe Info.cs *.dll
