# Makefile for NyFolder
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
NAME = Talk
VERSION = 0.1
# ==========================================================================

SRC = 	PluginInfo.cs	\
		Talk.cs			\
		TalkEditor.cs	\
		TalkManager.cs	\
		TalkWindow.cs

GLADE_RES = 
IMG_RES = 	Pixmaps/TalkBubble.png	\
			Pixmaps/TalkPopup.xpm

# Sources & Resources
RESOURCES = $(IMG_RES) $(GLADE_RES)

TalkPlugin: PluginInfo $(SRC) $(RESOURCES)
	@echo
	@echo "$(NAME) Plugin (DLL)"
	@echo "======================================================="
	$(CC) $(CFLAGS) -target:library $(PKGS) $(SRC) $(RESOURCES:%=-resource:%) -out:nyfolder-talk.dll
	@echo "======================================================="
	@echo

PluginInfo: PluginInfo.cs.in
	@echo
	@echo "Name: $(NAME)"
	@echo "Version: $(VERSION)"
	@echo "======================================================="
	sed -e "s|\@version\@|${VERSION}|" 			\
	    -e "s|\@name\@|${NAME}|" 				\
		 PluginInfo.cs.in > PluginInfo.cs
	@echo "======================================================="
	@echo

install: nyfolder-talk.dll
	mv nyfolder-talk.dll ~/.nyFolder/Plugins/

clean: 
	rm -f *.exe Info.cs *.dll
