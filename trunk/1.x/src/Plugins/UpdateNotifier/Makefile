# Makefile for NyFolder Plugin
# Author: Matteo Bertozzi <theo.bertozzi@gmail.com>
# ===================================================

CC = gmcs
CFLAGS = 

PKG_GTK = -pkg:gtk-sharp-2.0
PKG_CAIRO = -r:Mono.Cairo.dll
PKG_NIRY = -r:../../niry-sharp.dll
PKG_NF_PLUGIN = -r:../../nyfolder-plugin.dll
PKGS = $(PKG_GTK) $(PKG_CAIRO) $(PKG_NIRY) $(PKG_NF_PLUGIN)

# ============================== Plugin Infos ====
NAME = Update Notifier
VERSION = 0.1
AUTHORS = \"Matteo Bertozzi\"
WEB = http://nyfolder.berlios.de
PRIORITY = 0
DESCRIPTION = NyFolder Update Notifier
COPYRIGHT = (C) Copyright Matteo Bertozzi 2006
COMPANY = Matteo Bertozzi
# ==================================================
PLUGIN_DLL = nfUpdateNotifier.dll

SRC_Base = 		AssemblyInfo.cs	\
				Info.cs
SRC_Plugin =	src/UpdateNotifier.cs	\
				src/UpdateInfo.cs

RES_Glade =	
RES_Images = 	Pixmaps/UpdateNotifier.png

SRC = $(SRC_Base) $(SRC_Plugin)
RESOURCES = $(RES_Glade) $(RES_Images)

Plugin: PluginInfo $(SRC) $(RESOURCES)
	@echo
	@echo "$(NAME) Plugin (DLL)"
	@echo "======================================================="
	$(CC) $(CFLAGS) -target:library $(PKGS) $(SRC) $(RESOURCES:%=-resource:%) -out:$(PLUGIN_DLL)
	@echo

PluginInfo: Info.cs.in AssemblyInfo.cs.in 
	@echo
	@echo "Name: $(NAME)"
	@echo "Version: $(VERSION)"
	@echo "Compiler: $(COMPILER)"
	@echo "======================================================="
	sed -e "s|\@name\@|${NAME}|" 				\
		-e "s|\@version\@|${VERSION}|" 			\
	    -e "s|\@authors\@|${AUTHORS}|" 			\
		-e "s|\@version\@|${WEB}|" 				\
	    -e "s|\@priority\@|${PRIORITY}|" 		\
		 Info.cs.in > Info.cs
	@echo "======================================================="
	@echo
	sed -e "s|\@name\@|${NAME}|" 				\
		-e "s|\@version\@|${VERSION}|" 			\
	    -e "s|\@description\@|${DESCRIPTION}|" 	\
		-e "s|\@copyright\@|${COPYRIGHT}|" 		\
	    -e "s|\@company\@|${COMPANY}|" 		\
		AssemblyInfo.cs.in > AssemblyInfo.cs
	@echo

install: $(PLUGIN_DLL)
	cp $(PLUGIN_DLL) ~/.nyFolder-1.x/Plugins/$(PLUGIN_DLL)

clean:
	rm -f $(PLUGIN_DLL) Info.cs AssemblyInfo.cs
