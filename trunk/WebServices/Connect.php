<?php # Connect.php
require_once("NyFolderDb.php");
require_once("utils.php");

# Get UserName & User Magic
$userName = VarRequest('user', 'UserName');
$userMagic = VarRequest('magic', 'Magic');
$userPort = VarRequest('port', 'Port');

$db = new NyFolderDb;
if ($db->Initialize() === true) {

	if ($db->Authenticate($userName, $userMagic) === false) {
		echo XmlOutput("error", "Authentication Failed, Invalid UserMagic");		
	} else if ($db->Connect($userName, $userPort) === true) {
		echo XmlOutput("connect", "Connection Ok");
	} else {
		echo XmlOutput("error", "Connection Error");
	}

	$db->Destroy();
} else {
	echo XmlOutput("error", "Database Connection Failed");
}
?>
