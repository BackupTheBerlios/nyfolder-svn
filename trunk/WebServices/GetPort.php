<?php # GetPort.php
require_once("NyFolderDb.php");
require_once("utils.php");

$userName = VarRequest('user', 'UserName');

# Get Port
$db = new NyFolderDb;
if ($db->Initialize() === true) {

	if (($userPort = $db->GetPort($userName)) !== false) {
		echo XmlOutput("port", $userPort);
	} else {
		echo XmlOutput("error", "Get User ".$userName." Port Failed");
	}

	$db->Destroy();
} else {
	echo "DB Not Connected <br />";
}
?>
