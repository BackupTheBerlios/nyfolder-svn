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
NAME = Talk
VERSION = 0.1
AUTHOR = Matteo Bertozzi
DLL_NAME = talk
# ==========================================================================

SRC = 	PluginInfo.cs		\
		Talk.cs				\
		TalkFrame.cs		\
		TalkManager.cs		\
		TalkView.cs			\
		TalkWithDialog.cs

IMG_RES = 	Pixmaps/TalkBubble.png			\
			Pixmaps/face-angel.png			\
			Pixmaps/face-crying.png			\
			Pixmaps/face-devil-grin.png		\
			Pixmaps/face-glasses.png		\
			Pixmaps/face-grin.png			\
			Pixmaps/face-kiss.png			\
			Pixmaps/face-plain.png			\
			Pixmaps/face-sad.png			\
			Pixmaps/face-smile-big.png		\
			Pixmaps/face-smile.png			\
			Pixmaps/face-surprise.png		\
			Pixmaps/face-wink.png

GLADE_RES =		Glade/TalkWithDialog.glade

# Sources & Resources
RESOURCES = $(IMG_RES) $(GLADE_RES)

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
