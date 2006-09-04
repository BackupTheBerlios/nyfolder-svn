#!/usr/bin/env python

import os

if (__name__ == '__main__'):
	for fileName in os.listdir("."):
		if (fileName[0].islower() == True):
			firstChar = fileName[0].upper()
			newFileName = "FileType" + firstChar + fileName[1:]
			os.rename(fileName, newFileName)
			print newFileName
		else:
			print fileName
