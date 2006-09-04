LOGIN_NAME="matteo_bertozzi"

export SVN_SSH="ssh -l ${LOGIN_NAME}"

svn -m "Initial Import" import svn+ssh://svn.berlios.de/svnroot/repos/nyfolder/trunk

#svn checkout https://matteo_bertozzi@svn.berlios.de/svnroot/repos/nyfolder/trunk
