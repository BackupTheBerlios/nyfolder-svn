# Makefile for NyFolder Plugin
# =================================================================================

CC = gmcs
CFLAGS = 

# ===================================== Packages ===========================
PKG_GTK = -pkg:gtk-sharp-2.0
PKG_GLADE = -pkg:glade-sharp-2.0
PKG_NIRY = -r:../../niry-sharp.dll
PKG_NYFOLDER_PLUGIN = -r:../../nyfolder-plugin.dll
PKGS = $(PKG_GTK) $(PKG_GLADE) $(PKG_NIRY) $(PKG_NYFOLDER_PLUGIN)
# ==========================================================================

# ===================================== NyFolder Info ======================
NAME = Search
VERSION = 0.1
AUTHOR = Matteo Bertozzi
# ==========================================================================

SRC = 	PluginInfo.cs		\
		Search.cs			\
		GUI/ResultViewer.cs	\
		GUI/TopBar.cs		\
		GUI/Window.cs

GLADE_RES = 
IMG_RES = 	Pixmaps/Search.png

# Sources & Resources
RESOURCES = $(IMG_RES) $(GLADE_RES)

TalkPlugin: PluginInfo $(SRC) $(RESOURCES)
	@echo
	@echo "$(NAME) Plugin (DLL)"
	@echo "======================================================="
	$(CC) $(CFLAGS) -target:library $(PKGS) $(SRC) $(RESOURCES:%=-resource:%) -out:nyfolder-search.dll
	@echo "======================================================="
	@echo

PluginInfo: PluginInfo.cs.in
	@echo
	@echo "Name: $(NAME)"
	@echo "Version: $(VERSION)"
	@echo "======================================================="
	sed -e "s|\@version\@|${VERSION}|" 			\
	    -e "s|\@name\@|${NAME}|" 				\
	    -e "s|\@author\@|${AUTHOR}|" 			\
		 PluginInfo.cs.in > PluginInfo.cs
	@echo "======================================================="
	@echo

install: nyfolder-search.dll
	mv nyfolder-search.dll ~/.nyFolder/Plugins/

clean: 
	rm -f *.exe PluginInfo.cs *.dll
