<?php # Disconnect.php
require_once("NyFolderDb.php");
require_once("utils.php");

# Get UserName & User Magic
$userName = VarRequest('user', 'UserName');
$userMagic = VarRequest('magic', 'Magic');

$db = new NyFolderDb;
if ($db->Initialize() === true) {

	if ($db->Authenticate($userName, $userMagic) === false) {
		echo XmlOutput("error", "Authentication Failed, Invalid UserMagic");		
	} else if ($db->Disconnect($userName) === true) {
		echo XmlOutput("disconnect", "Disconnection Ok");
	} else {
		echo XmlOutput("error", "Disconnection Error");
	}

	$db->Destroy();
} else {
	echo XmlOutput("error", "Database Connection Failed");
}
?>
