<?php # GetIp.php
require_once("NyFolderDb.php");
require_once("utils.php");

$userName = VarRequest('user', 'UserName');

# Get Ip
$db = new NyFolderDb;
if ($db->Initialize() === true) {

	if (($userIp = $db->GetIp($userName)) !== false) {
		echo XmlOutput("ip", $userIp);
	} else {
		echo XmlOutput("error", "Get User ".$userName." Ip Failed");
	}

	$db->Destroy();
} else {
	echo "DB Not Connected <br />";
}
?>
