<?php # Login.php
require_once("NyFolderDb.php");
require_once("utils.php");

# Get UserName
$userName = VarRequest('user', 'UserName');
$passWord = VarRequest('passwd', 'PassWord');

$db = new NyFolderDb;
if ($db->Initialize() === true) {
	if (($magic = $db->Login($userName, md5($passWord))) !== false) {
		echo XmlOutput("login", $magic, "ip=" . XmlQuote(RealIp()));
	} else {
		if (strpos(mysql_error(), "Duplicate entry") !== false) {
			echo XmlOutput("error", "User '" . $userName . "' Already Logged In");
		} else {
			echo XmlOutput("error", "Login Failed, Wrong Username or Password");
		}
	}

	$db->Destroy();
} else {
	echo XmlOutput("error", "Database Connection Failed");
}
?>
