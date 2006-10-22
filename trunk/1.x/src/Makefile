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

SRC_GUI_Dialogs =	GUI/Dialogs/Login/Login.cs				\
					GUI/Dialogs/Login/MenuManager.cs		\
					GUI/Dialogs/Login/ProgressDialog.cs

SRC_GUI_Main =

SRC_GUI_Glue = 	

SRC_GUI_Base =	GUI/Base/UIManager.cs


SRC_GUI = $(SRC_GUI_Base) $(SRC_GUI_Main) $(SRC_GUI_Dialogs) $(SRC_GUI_Glue)

SRC_Protocol = 	

SRC_PluginLib =	PluginLib/Plugin.cs			\
				PluginLib/INyFolder.cs		\
				PluginLib/PluginInfo.cs		\
				PluginLib/PluginManager.cs

SRC_Utils = 	Utils/Paths.cs	\
				Utils/Proxy.cs

RES_Glade =	
RES_Images = 	

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
