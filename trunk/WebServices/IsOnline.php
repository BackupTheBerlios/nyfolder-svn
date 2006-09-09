<?php
require_once("NyFolderDb.php");
require_once("utils.php");

# Get UserName
$userName = VarRequest('user', 'UserName');

$db = new NyFolderDb;
if ($db->Initialize() === true) {

	if ($db->CheckUpdate($userName) === false) {
		echo XmlOutput("response", "User is Offline (Last Update Outdated)");
	} else if ($db->GetStatus($userName) == 1) {
		echo XmlOutput("response", "User is Online");
	} else {
		echo XmlOutput("response", "User is Offline");
	}

	$db->Destroy();
} else {
	echo XmlOutput("error", "Database Connection Failed");
}
?>
