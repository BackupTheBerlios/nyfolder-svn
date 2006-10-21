# Makefile for NyFolder
# Author: Matteo Bertozzi <theo.bertozzi@gmail.com>
# ===================================================

CC = gmcs
CFLAGS = 

PKG_GTK = -pkg:gtk-sharp-2.0
PKG_CAIRO = -r:Mono.Cairo.dll
PKG_GLADE = -pkg:glade-sharp-2.0
PKG_SQLITE = -r:System.Data.dll -r:Mono.Data.SqliteClient.dll
PKG_NIRY = -r:niry-sharp.dll
PKGS = $(PKG_GTK) $(PKG_CAIRO) $(PKG_GLADE) $(PKG_SQLITE) $(PKG_NIRY)

# ============================== NyFolder Infos ====
NAME = NyFolder
VERSION = 1.0.0
COMPILER = `$(CC) --version`
# ==================================================
NYFOLDER_DLL = nyfolder-plugin.dll

SRC_Base = 		AssemblyInfo.cs	\
				Info.cs				\

SRC_GUI_Dialogs =	GUI/Dialogs/About.cs			\
					GUI/Dialogs/AcceptUser.cs		\
					GUI/Dialogs/AddPeer.cs			\
					GUI/Dialogs/Login.cs			\
					GUI/Dialogs/ProxySettings.cs	\
					GUI/Dialogs/RemovePeer.cs		\
					GUI/Dialogs/SetPort.cs

SRC_GUI_Glue = 	GUI/Glue/Dialogs.cs				\
				GUI/Glue/FolderManager.cs		\
				GUI/Glue/NetworkManager.cs		\
				GUI/Glue/ProtocolManager.cs

SRC_GUI_Base =	GUI/Dnd.cs				\
				GUI/FolderStore.cs		\
				GUI/FolderViewer.cs		\
				GUI/MenuManager.cs		\
				GUI/NetworkStore.cs		\
				GUI/NetworkViewer.cs	\
				GUI/NotebookViewer.cs	\
				GUI/StockIcons.cs		\
				GUI/TabLabel.cs			\
				GUI/UserPanel.cs		\
				GUI/Window.cs

SRC_GUI = $(SRC_GUI_Base) $(SRC_GUI_Dialogs) $(SRC_GUI_Glue)

SRC_Protocol = 	Protocol/CmdManager.cs			\
				Protocol/DownloadManager.cs		\
				Protocol/FileReceiver.cs		\
				Protocol/FileSender.cs			\
				Protocol/HttpRequest.cs			\
				Protocol/Login.cs				\
				Protocol/MyInfo.cs				\
				Protocol/UploadManager.cs		\
				Protocol/UserInfo.cs			\
				Protocol/XmlRequest.cs

SRC_PluginLib =	PluginLib/Plugin.cs			\
				PluginLib/INyFolder.cs		\
				PluginLib/PluginInfo.cs		\
				PluginLib/PluginManager.cs

SRC_Utils = 	Utils/Paths.cs	\
				Utils/Proxy.cs

RES_Glade =	Glade/AcceptUserDialog.glade		\
			Glade/AddPeerDialog.glade			\
			Glade/RemovePeerDialog.glade		\
			Glade/SetPortDialog.glade			\
			Glade/ProxySettingsDialog.glade

RES_Images = 	Pixmaps/Channel.png					\
				Pixmaps/Close.png						\
				Pixmaps/Directory.png					\
				Pixmaps/Download.png					\
				Pixmaps/FileTypes/FileTypeAc3.png		\
				Pixmaps/FileTypes/FileTypeAiff.png		\
				Pixmaps/FileTypes/FileTypeArj.png		\
				Pixmaps/FileTypes/FileTypeAu.png		\
				Pixmaps/FileTypes/FileTypeBak.png		\
				Pixmaps/FileTypes/FileTypeBin.png		\
				Pixmaps/FileTypes/FileTypeBmp.png		\
				Pixmaps/FileTypes/FileTypeBz2.png		\
				Pixmaps/FileTypes/FileTypeC.png		\
				Pixmaps/FileTypes/FileTypeClass.png	\
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
				Pixmaps/FileTypes/FileTypeH.png		\
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
				Pixmaps/FileTypes/FileTypeZip.png		\
				Pixmaps/InsecureAuth.png				\
				Pixmaps/Lock.png						\
				Pixmaps/Logout.png						\
				Pixmaps/MyFolder.png					\
				Pixmaps/MyFolderOffline.png			\
				Pixmaps/MyFolderOnline.png				\
				Pixmaps/NFSplash.png					\
				Pixmaps/Network.png					\
				Pixmaps/NetworkInsecure.png			\
				Pixmaps/NyFolderIcon.png				\
				Pixmaps/NyFolderLogo.png				\
				Pixmaps/NyFolderSmall.png				\
				Pixmaps/Proxy.png						\
				Pixmaps/SecureAuth.png					\
				Pixmaps/Send.png						\
				Pixmaps/StockMyFolder.png				\
				Pixmaps/StockNetwork.png				\
				Pixmaps/StockNetwork2.png				\
				Pixmaps/MyFolderOfflineInsecure.png	\
				Pixmaps/MyFolderOnlineInsecure.png

SRC = $(SRC_Base) $(SRC_GUI) $(SRC_Protocol) $(SRC_PluginLib) $(SRC_Utils)
RESOURCES = $(RES_Glade) $(RES_Images)

NyFolder: Main.cs NyFolderApp.cs NyFolderPlugin
	@echo
	@echo "$(NAME) Main Application"
	@echo "======================================================="
	$(CC) $(CFLAGS) -target:winexe $(PKGS) Main.cs NyFolderApp.cs -out:NyFolder.exe -r:$(NYFOLDER_DLL) -resource:Pixmaps/NFSplash.png 

NyFolderPlugin: NyFolderInfo $(SRC) $(RESOURCES)
	@echo
	@echo "$(NAME) Plugin (DLL)"
	@echo "======================================================="
	$(CC) $(CFLAGS) -target:library $(PKGS) $(SRC) $(RESOURCES:%=-resource:%) -out:$(NYFOLDER_DLL)
	@echo "======================================================="
	@echo "$(CC) -target:library MyPlugin.cs -r:$(NYFOLDER_DLL)"
	@echo "======================================================="
	@echo

NyFolderInfo: Info.cs.in AssemblyInfo.cs.in 
	@echo
	@echo "Name: $(NAME)"
	@echo "Version: $(VERSION)"
	@echo "Compiler: $(COMPILER)"
	@echo "======================================================="
	sed -e "s|\@name\@|${NAME}|" 				\
		-e "s|\@version\@|${VERSION}|" 			\
	    -e "s|\@compiler\@|${COMPILER}|" 		\
		 Info.cs.in > Info.cs
	@echo "======================================================="
	@echo
	sed -e "s|\@version\@|${VERSION}|" AssemblyInfo.cs.in > AssemblyInfo.cs
	@echo

clean:
	rm -f $(NYFOLDER_DLL) NyFolder.exe Info.cs AssemblyInfo.cs
