# Makefile for NyFolder Plugin
# =================================================================================

CC = gmcs
CFLAGS = 

# ===================================== Packages ===========================
PKG_GTK = -pkg:gtk-sharp-2.0
PKG_NIRY = -r:../../niry-sharp.dll
PKG_NYFOLDER_PLUGIN = -r:../../nyfolder-plugin.dll
PKGS = $(PKG_GTK) $(PKG_NIRY) $(PKG_NYFOLDER_PLUGIN)
# ==========================================================================

# ===================================== NyFolder Info ======================
NAME = 
VERSION = 0.1
AUTHOR = Matteo Bertozzi
DLL_NAME = 
# ==========================================================================

SRC = 	PluginInfo.cs				\

IMG_RES = 	

# Sources & Resources
RESOURCES = $(IMG_RES)

TalkPlugin: PluginInfo $(SRC) $(RESOURCES)
	@echo
	@echo "$(NAME) Plugin (DLL)"
	@echo "======================================================="
	$(CC) $(CFLAGS) -target:library $(PKGS) $(SRC) $(RESOURCES:%=-resource:%) -out:nyfolder-$(DLL_NAME).dll
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

install: nyfolder-$(DLL_NAME).dll
	mv nyfolder-$(DLL_NAME).dll ~/.nyFolder/Plugins/

clean: 
	rm -f *.exe PluginInfo.cs *.dll
